using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThesisDB.Models;
using Google.GenAI;
using Google.GenAI.Types;

namespace ThesisDB.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ThesisDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private const string ContainerName = "thesis-pdf";
        private readonly Client _geminiClient;
        
        public ReviewController(ThesisDbContext context, BlobServiceClient blobServiceClient, Client geminiClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
            _geminiClient = geminiClient;
        }

        // GET: Review/Create/5 (5 is ThesisId)
        public async Task<IActionResult> Create(int? thesisId)
        {
            if (thesisId == null)
            {
                return NotFound();
            }

            var thesis = await _context.Theses.FindAsync(thesisId);
            if (thesis == null)
            {
                return NotFound();
            }

            var review = new Review { ThesisId = thesis.Id };
            ViewBag.ThesisTitle = thesis.Title;
            ViewBag.HasPdf = !string.IsNullOrEmpty(thesis.PdfFileName);
            PopulateGradeSelectList();
            return View(review);
        }

        // POST: Review/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Summary,Strengths,Weaknesses,Evaluation,ContentVal,LayoutVal,StructureVal,StyleVal,LiteratureVal,DifficultyVal,NoveltyVal,RichnessVal,ContentWt,LayoutWt,StructureWt,StyleWt,LiteratureWt,DifficultyWt,NoveltyWt,RichnessWt,Grade,ThesisId")] Review review)
        {
            ModelState.Remove("Thesis");
            
            if (ModelState.IsValid)
            {
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Thesis");
            }
            
            var thesis = await _context.Theses.FindAsync(review.ThesisId);
            if(thesis != null)
            {
                ViewBag.ThesisTitle = thesis.Title;
                ViewBag.HasPdf = !string.IsNullOrEmpty(thesis.PdfFileName);
            }
            
            PopulateGradeSelectList(review.Grade);
            return View(review);
        }

        // GET: Review/Edit/5 (5 is ReviewId)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.Include(r => r.Thesis).FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }
            ViewBag.ThesisTitle = review.Thesis.Title;
            ViewBag.HasPdf = !string.IsNullOrEmpty(review.Thesis.PdfFileName);
            PopulateGradeSelectList(review.Grade);
            return View(review);
        }

        // POST: Review/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Summary,Strengths,Weaknesses,Evaluation,ContentVal,LayoutVal,StructureVal,StyleVal,LiteratureVal,DifficultyVal,NoveltyVal,RichnessVal,ContentWt,LayoutWt,StructureWt,StyleWt,LiteratureWt,DifficultyWt,NoveltyWt,RichnessWt,Grade,ThesisId")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Thesis");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Thesis");
            }
            
            var thesis = await _context.Theses.FindAsync(review.ThesisId);
            if(thesis != null)
            {
                ViewBag.ThesisTitle = thesis.Title;
                ViewBag.HasPdf = !string.IsNullOrEmpty(thesis.PdfFileName);
            }
            
            PopulateGradeSelectList(review.Grade);
            return View(review);
        }
        
        [HttpPost]
        public async Task<IActionResult> GenerateAiReview(int thesisId)
        {
            try
            {
                var thesis = await _context.Theses.FindAsync(thesisId);
                if (thesis == null || string.IsNullOrEmpty(thesis.PdfFileName))
                {
                    return BadRequest("Für diese Abschlussarbeit ist keine PDF-Datei vorhanden.");
                }

                var blobContainer = _blobServiceClient.GetBlobContainerClient(ContainerName);
                var blobClient = blobContainer.GetBlobClient(thesis.PdfFileName);

                if (!await blobClient.ExistsAsync())
                {
                    return BadRequest("Die PDF-Datei konnte nicht im Speicher gefunden werden.");
                }

                using var stream = new MemoryStream();
                await blobClient.DownloadToAsync(stream);
                var pdfBytes = stream.ToArray();

                var uploadedFile = await _geminiClient.Files.UploadAsync(pdfBytes, "thesis.pdf", new UploadFileConfig() { MimeType = "application/pdf" });
                
                var prompt = "Analysiere die beigefügte Abschlussarbeit und gib eine strukturierte Bewertung ab. Deine Antwort soll jeweils in einem Absatz eine Zusammenfassung (summary) der Arbeit, deren Stärken (strengths), deren Schwächen (weaknesses) sowie eine abschließende Bewertung inklusive eines Notenvorschlags (evaluation) beinhalten";

                Content contents = new Content() { Parts = new List<Part>() };
                contents.Parts.Add(new Part() { Text = prompt });
                contents.Parts.Add(new Part() { FileData = new FileData() { FileUri = uploadedFile.Uri, MimeType = "application/pdf" } });

                Schema reviewInfo = new() {
                    Properties = new Dictionary<string, Schema> {
                        { "summary", new Schema { Type = Google.GenAI.Types.Type.String , Title = "summary" } },
                        { "strengths", new Schema { Type = Google.GenAI.Types.Type.String , Title = "strengths" } },
                        { "weaknesses", new Schema { Type = Google.GenAI.Types.Type.String , Title = "weaknesses" } },
                        { "evaluation", new Schema { Type = Google.GenAI.Types.Type.String , Title = "evaluation" } }
                    },
                    PropertyOrdering = ["summary", "strengths", "weaknesses", "evaluation"],
                    Required = ["summary", "strengths", "weaknesses", "evaluation"],
                    Title = "reviewInfo",
                    Type = Google.GenAI.Types.Type.Object
                };

                GenerateContentConfig config = new() {
                    ResponseSchema = reviewInfo,
                    ResponseMimeType = "application/json"
                };

                var response = await _geminiClient.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash",
                    contents: contents,
                    config: config);
                
                return Ok(response.Text);
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using a logging framework)
                return StatusCode(500, $"Ein interner Fehler ist aufgetreten: {ex.Message}");
            }
        }

        // POST: Review/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Thesis");
        }

        // GET: Review/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Thesis)
                .ThenInclude(t => t.Student)
                .Include(r => r.Thesis)
                .ThenInclude(t => t.Supervisor)
                .Include(r => r.Thesis)
                .ThenInclude(t => t.Programme)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }

        private void PopulateGradeSelectList(double? selectedGrade = null)
        {
            var grades = new[] { 1.0, 1.3, 1.7, 2.0, 2.3, 2.7, 3.0, 3.3, 3.7, 4.0, 5.0 };
            var gradeList = grades.Select(g => new SelectListItem
            {
                Value = g.ToString("0.#", new System.Globalization.CultureInfo("de-DE")),
                Text = g.ToString("0.0", new System.Globalization.CultureInfo("de-DE")),
                Selected = selectedGrade.HasValue && System.Math.Abs(selectedGrade.Value - g) < 0.01
            }).ToList();
            
            ViewBag.GradeList = gradeList;
        }
    }
}