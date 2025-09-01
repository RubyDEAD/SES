using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SES.Data;
using SES.Models;

namespace SES.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext _db;
        public StudentsController(SchoolContext db) => _db = db;

        // List students (read-only query)
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var data = await _db.Students
                .Include(s => s.Enrollments).ThenInclude(e => e.Course)
                .AsNoTracking()
                .ToListAsync();
            return View(data);
        }

        // Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Student s)
        {
            if (!ModelState.IsValid) return View(s);
            _db.Students.Add(s);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Edit
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var s = await _db.Students.FindAsync(id);
            if (s == null) return NotFound();
            return View(s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Student s)
        {
            if (id != s.Id) return BadRequest();
            if (!ModelState.IsValid) return View(s);

            _db.Entry(s).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Delete
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await _db.Students.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (s == null) return NotFound();
            return View(s);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var s = await _db.Students.FindAsync(id);
            if (s != null)
            {
                _db.Students.Remove(s);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
