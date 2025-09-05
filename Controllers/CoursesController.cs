using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SES.Data;
using SES.Models;
using SES.Models.ViewModels;

namespace SES.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CoursesController : Controller
    {
        private readonly SchoolContext _db;

        public CoursesController(SchoolContext db) => _db = db;

        public async Task<IActionResult> AdminIndex()
        {
            var rows = await _db.Courses
                .Select(c => new CourseRowVm
                {
                    Id = c.Id,
                    Title = c.Title,
                    Credits = c.Credits,
                    MaxStudents = c.MaxEnrollies,
                    Department = c.Department,
                    EnrolledCount = _db.Enrollments.Count(e => e.CourseId == c.Id)
                })
                .AsNoTracking()
                .ToListAsync();

            var vm = new CoursesAdminVm
            {
                Courses = rows,
                NewCourse = new CourseEditVm()
            };
            return View(vm); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(Prefix = "NewCourse")] CourseEditVm vm)
        {
            if (!ModelState.IsValid)
            {
                var rows = await _db.Courses
                    .Select(c => new CourseRowVm
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Credits = c.Credits,
                        MaxStudents = c.MaxEnrollies,
                        Department = c.Department,
                        EnrolledCount = _db.Enrollments.Count(e => e.CourseId == c.Id)
                    })
                    .AsNoTracking()
                    .ToListAsync();
                return View("AdminIndex", new CoursesAdminVm { Courses = rows, NewCourse = vm });
            }

            _db.Courses.Add(new Course
            {
                Title = vm.Title,
                Credits = vm.Credits,
                Department = vm.Department,
                MaxEnrollies = vm.MaxStudents
            });
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(AdminIndex));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseEditVm vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return RedirectToAction(nameof(AdminIndex));

            var c = await _db.Courses.FindAsync(id);
            if (c == null) return NotFound();

            c.Title = vm.Title;
            c.Credits = vm.Credits;
            c.Department = vm.Department;
            c.MaxEnrollies = vm.MaxStudents;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(AdminIndex));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _db.Courses.FindAsync(id);
            if (c != null)
            {
                var hasEnrollments = await _db.Enrollments.AnyAsync(e => e.CourseId == id);
                if (!hasEnrollments)
                {
                    _db.Courses.Remove(c);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    TempData["Error"] = "Cannot delete a course with active enrollments.";
                }
            }
            return RedirectToAction(nameof(AdminIndex));
        }

        // NEW: View Students in a Course
        [HttpGet]
        public async Task<IActionResult> ViewStudents(int id)
        {
            var course = await _db.Courses
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null) return NotFound();

            var vm = new CourseStudentsVm
            {
                CourseId = course.Id,
                CourseTitle = course.Title,
                Students = course.Enrollments.Select(e => new StudentRowVm
                {
                    Id = e.Student.Id,
                    Name = e.Student.FirstName + " " + e.Student.LastName
                }).ToList() 
            };

            return View(vm); 
        }

        // Optional helper
        private int? GetCurrentUserId()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(id, out var uid) ? uid : null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStudent(int courseId, int studentId)
        {
        var enrollment = await _db.Enrollments
            .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == studentId);

        if (enrollment != null)
        {
        _db.Enrollments.Remove(enrollment);
        await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(ViewStudents), new { id = courseId });
}
        }
}
