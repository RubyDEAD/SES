using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SES.Data;
using SES.Models;

namespace SES.Controllers
{
    public class CoursesController : Controller
    {
        private readonly SchoolContext _db;

        public CoursesController(SchoolContext db) => _db = db;

        // Anyone can view courses
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var data = await _db.Courses
                .Include(c => c.Enrollments).ThenInclude(e => e.Student)
                .AsNoTracking()
                .ToListAsync();
            return View(data);
        }

        // Only Admin can access create
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Course c)
        {
            if (!ModelState.IsValid) return View(c);
            _db.Add(c);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Only Admin can edit
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var c = await _db.Courses.FindAsync(id);
            if (c == null) return NotFound();
            return View(c);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Course c)
        {
            if (id != c.Id) return BadRequest();
            if (!ModelState.IsValid) return View(c);

            _db.Entry(c).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Only Admin can delete
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _db.Courses.FindAsync(id);
            if (c == null) return NotFound();
            return View(c);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var c = await _db.Courses.FindAsync(id);
            if (c != null)
            {
                _db.Courses.Remove(c);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper: get current UserId if you later need row-level filtering
        private int? GetCurrentUserId()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(id, out var uid) ? uid : null;
        }
    }
}
