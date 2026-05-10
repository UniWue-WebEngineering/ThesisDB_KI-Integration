using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThesisDB.Models;

namespace ThesisDB.Controllers
{
    public class ThesisController : Controller
    {
        private readonly ThesisDbContext _context;

        public ThesisController(ThesisDbContext context)
        {
            _context = context;
        }

        // GET: Thesis
        public async Task<IActionResult> Index()
        {
            var thesisDbContext = _context.Theses.Include(t => t.Programme).Include(t => t.Student).Include(t => t.Supervisor);
            return View(await thesisDbContext.ToListAsync());
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
            ViewData["ProgrammeId"] = new SelectList(_context.Programmes, "Id", "Name");
            var students = _context.Students.Select(s => new { Id = s.Id, DisplayName = s.LastName + ", " + s.FirstName + " (" + s.MatriculationNumber + ")" }).ToList();
            ViewData["StudentId"] = new SelectList(students, "Id", "DisplayName");
            var supervisors = _context.Supervisors.Select(s => new { Id = s.Id, DisplayName = s.LastName + ", " + s.FirstName }).ToList();
            ViewData["SupervisorId"] = new SelectList(supervisors, "Id", "DisplayName");
            return View();
        }

        // POST: Thesis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Status,Type,StartDate,EndDate,ProgrammeId,StudentId,SupervisorId")] Thesis thesis)
        {
            // Remove validation errors for navigation properties and auto-set fields
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
            ViewData["ProgrammeId"] = new SelectList(_context.Programmes, "Id", "Name", thesis.ProgrammeId);
            var students = _context.Students.Select(s => new { Id = s.Id, DisplayName = s.LastName + ", " + s.FirstName + " (" + s.MatriculationNumber + ")" }).ToList();
            ViewData["StudentId"] = new SelectList(students, "Id", "DisplayName", thesis.StudentId);
            var supervisors = _context.Supervisors.Select(s => new { Id = s.Id, DisplayName = s.LastName + ", " + s.FirstName }).ToList();
            ViewData["SupervisorId"] = new SelectList(supervisors, "Id", "DisplayName", thesis.SupervisorId);
            return View(thesis);
        }

        // GET: Thesis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thesis = await _context.Theses.FindAsync(id);
            if (thesis == null)
            {
                return NotFound();
            }
            ViewData["ProgrammeId"] = new SelectList(_context.Programmes, "Id", "Name", thesis.ProgrammeId);
            var students = _context.Students.Select(s => new { Id = s.Id, DisplayName = s.LastName + ", " + s.FirstName + " (" + s.MatriculationNumber + ")" }).ToList();
            ViewData["StudentId"] = new SelectList(students, "Id", "DisplayName", thesis.StudentId);
            var supervisors = _context.Supervisors.Select(s => new { Id = s.Id, DisplayName = s.LastName + ", " + s.FirstName }).ToList();
            ViewData["SupervisorId"] = new SelectList(supervisors, "Id", "DisplayName", thesis.SupervisorId);
            return View(thesis);
        }

        // POST: Thesis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Status,Type,StartDate,EndDate,ProgrammeId,StudentId,SupervisorId")] Thesis thesis)
        {
            if (id != thesis.Id)
            {
                return NotFound();
            }

            // Remove validation errors for navigation properties and auto-set fields
            ModelState.Remove("Programme");
            ModelState.Remove("Student");
            ModelState.Remove("Supervisor");
            ModelState.Remove("Review");

            if (ModelState.IsValid)
            {
                try
                {
                    thesis.LastModified = DateTime.Now;
                    _context.Update(thesis);
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
            ViewData["ProgrammeId"] = new SelectList(_context.Programmes, "Id", "Name", thesis.ProgrammeId);
            var students = _context.Students.Select(s => new { Id = s.Id, DisplayName = s.LastName + ", " + s.FirstName + " (" + s.MatriculationNumber + ")" }).ToList();
            ViewData["StudentId"] = new SelectList(students, "Id", "DisplayName", thesis.StudentId);
            var supervisors = _context.Supervisors.Select(s => new { Id = s.Id, DisplayName = s.LastName + ", " + s.FirstName }).ToList();
            ViewData["SupervisorId"] = new SelectList(supervisors, "Id", "DisplayName", thesis.SupervisorId);
            return View(thesis);
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
    }
}
