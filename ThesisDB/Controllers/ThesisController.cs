using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThesisDB.Models;

namespace ThesisDB.Controllers
{
    public class ThesisController : Controller
    {
        private readonly ThesisDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private const string ContainerName = "thesis-pdf";

        public ThesisController(ThesisDbContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        // GET: Thesis
        public async Task<IActionResult> Index(string searchString, int? pageNumber, int? pageSize)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["PageSize"] = pageSize ?? 10;

            var theses = _context.Theses
                .Include(t => t.Programme)
                .Include(t => t.Student)
                .Include(t => t.Supervisor)
                .Include(t => t.Review)
                .AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                theses = theses.Where(s => s.Title.Contains(searchString)
                                       || (s.Student != null && (s.Student.FirstName.Contains(searchString) || s.Student.LastName.Contains(searchString))));
            }

            int finalPageSize = pageSize ?? 10;
            int finalPageNumber = pageNumber ?? 1;

            var pagedTheses = await PaginatedList<Thesis>.CreateAsync(theses.AsNoTracking(), finalPageNumber, finalPageSize);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ThesisListGroupPartial", pagedTheses);
            }

            return View(pagedTheses);
        }

        // GET: Thesis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thesis = await _context.Theses
                .Include(t => t.Programme)
                .Include(t => t.Student)
                .Include(t => t.Supervisor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (thesis == null)
            {
                return NotFound();
            }

            return View(thesis);
        }

        // GET: Thesis/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: Thesis/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Status,Type,StartDate,EndDate,ProgrammeId,StudentId,SupervisorId")] Thesis thesis)
        {
            ModelState.Remove("Programme");
            ModelState.Remove("Student");
            ModelState.Remove("Supervisor");
            ModelState.Remove("Review");

            if (ModelState.IsValid)
            {
                thesis.LastModified = DateTime.Now;
                _context.Add(thesis);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(thesis);
            return View(thesis);
        }

        // GET: Thesis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thesis = await _context.Theses
                .Include(t => t.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (thesis == null)
            {
                return NotFound();
            }
            PopulateDropdowns(thesis);
            return View(thesis);
        }

        // POST: Thesis/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Status,Type,StartDate,EndDate,ProgrammeId,StudentId,SupervisorId,PdfFileName")] Thesis thesis, IFormFile pdfFile)
        {
            if (id != thesis.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Programme");
            ModelState.Remove("Student");
            ModelState.Remove("Supervisor");
            ModelState.Remove("Review");
            ModelState.Remove("pdfFile");

            if (pdfFile != null)
            {
                if (pdfFile.ContentType != "application/pdf")
                {
                    ModelState.AddModelError("pdfFile", "Es sind nur PDF-Dateien erlaubt.");
                }
                if (pdfFile.Length > 10 * 1024 * 1024) // 10 MB
                {
                    ModelState.AddModelError("pdfFile", "Die Datei darf maximal 10 MB groß sein.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var thesisToUpdate = await _context.Theses.FindAsync(id);
                    if (thesisToUpdate == null)
                    {
                        return NotFound();
                    }

                    if (pdfFile != null)
                    {
                        var blobContainer = _blobServiceClient.GetBlobContainerClient(ContainerName);
                        await blobContainer.CreateIfNotExistsAsync(PublicAccessType.None);

                        var fileName = $"{Guid.NewGuid().ToString().Replace("-", "")}{Path.GetExtension(pdfFile.FileName)}";
                        var blobClient = blobContainer.GetBlobClient(fileName);

                        await blobClient.UploadAsync(pdfFile.OpenReadStream(), true);
                        thesisToUpdate.PdfFileName = fileName;
                    }

                    thesisToUpdate.Title = thesis.Title;
                    thesisToUpdate.Description = thesis.Description;
                    thesisToUpdate.Status = thesis.Status;
                    thesisToUpdate.Type = thesis.Type;
                    thesisToUpdate.StartDate = thesis.StartDate;
                    thesisToUpdate.EndDate = thesis.EndDate;
                    thesisToUpdate.ProgrammeId = thesis.ProgrammeId;
                    thesisToUpdate.StudentId = thesis.StudentId;
                    thesisToUpdate.SupervisorId = thesis.SupervisorId;
                    thesisToUpdate.LastModified = DateTime.Now;

                    _context.Update(thesisToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThesisExists(thesis.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            if (thesis.StudentId.HasValue)
            {
                thesis.Student = await _context.Students.FindAsync(thesis.StudentId.Value);
            }
            PopulateDropdowns(thesis);
            return View(thesis);
        }

        public async Task<IActionResult> DownloadPdf(int id)
        {
            var thesis = await _context.Theses
                .Include(t => t.Student)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (thesis == null || string.IsNullOrEmpty(thesis.PdfFileName))
            {
                return NotFound();
            }

            var blobContainer = _blobServiceClient.GetBlobContainerClient(ContainerName);
            var blobClient = blobContainer.GetBlobClient(thesis.PdfFileName);

            if (!await blobClient.ExistsAsync())
            {
                return NotFound();
            }

            var stream = new MemoryStream();
            await blobClient.DownloadToAsync(stream);
            stream.Position = 0;

            var studentLastName = thesis.Student?.LastName ?? "Student";
            var sanitizedLastName = Regex.Replace(studentLastName, @"[^a-zA-Z0-9_]", "");
            var downloadFileName = $"Thesis_{sanitizedLastName}.pdf";

            return File(stream, "application/pdf", downloadFileName);
        }

        // GET: Thesis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thesis = await _context.Theses
                .Include(t => t.Programme)
                .Include(t => t.Student)
                .Include(t => t.Supervisor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (thesis == null)
            {
                return NotFound();
            }

            return View(thesis);
        }

        // POST: Thesis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thesis = await _context.Theses.FindAsync(id);
            if (thesis != null)
            {
                _context.Theses.Remove(thesis);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThesisExists(int id)
        {
            return _context.Theses.Any(e => e.Id == id);
        }

        private void PopulateDropdowns(Thesis thesis = null)
        {
            ViewData["ProgrammeId"] = new SelectList(_context.Programmes, "Id", "Name", thesis?.ProgrammeId);
            var supervisors = _context.Supervisors.Select(s => new { Id = s.Id, DisplayName = s.LastName + ", " + s.FirstName }).ToList();
            ViewData["SupervisorId"] = new SelectList(supervisors, "Id", "DisplayName", thesis?.SupervisorId);
        }
    }
}