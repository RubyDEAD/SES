using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SES.Data;
using SES.Models.ViewModels;

[Authorize]
public class StudentsController : Controller
{
    private readonly SchoolContext _db;
    public StudentsController(SchoolContext db) => _db = db;

    public async Task<IActionResult> Profile(int id)
    {
        var student = await _db.Students
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id == id);

        if (student == null) return NotFound();

        // Only allow the logged-in student to access their own profile
        var userStudentId = User.Claims.FirstOrDefault(c => c.Type == "StudentId")?.Value;
        if (userStudentId == null || userStudentId != id.ToString())
        {
            return Forbid();
        }

        var vm = new StudentProfileVm
        {
            StudentId = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Since = student.DateCreated,
            Courses = student.Enrollments
                .Select(e => new CourseItem
                {
                    CourseId = e.CourseId,
                    Title = e.Course.Title,
                    Credits = e.Course.Credits,
                    Grade = e.Grade,
                    EnrolledOn = e.EnrolledOn, // Add this line
                    MaxStudents = e.Course.MaxEnrollies,
                    EnrolledCount = 0
                })
                .ToList()
        };

        return View(vm);
    }
}
