using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SES.Data;
using SES.Models;
using SES.Models.ViewModels;


[Authorize]
public class EnrollmentsController : Controller
{
    private readonly SchoolContext _db;
    public EnrollmentsController(SchoolContext db) => _db = db;

    // GET: /Enrollments
    public async Task<IActionResult> Index()
    {
        var studentIdStr = User.Claims.FirstOrDefault(c => c.Type == "StudentId")?.Value;
        if (!int.TryParse(studentIdStr, out var studentId))
            return Forbid();

        var enrollments = await _db.Enrollments
            .Include(e => e.Course)
            .Include(e => e.Student)
            .Where(e => e.StudentId == studentId)
            .ToListAsync();

        var enrolledCourseIds = enrollments.Select(e => e.CourseId).ToList();

        var availableCourses = await _db.Courses
            .Where(c => !enrolledCourseIds.Contains(c.Id))
            .Select(c => new CourseItem
            {
                CourseId = c.Id,
                Title = c.Title,
                Credits = c.Credits,
                Department = c.Department,
                EnrolledCount = _db.Enrollments.Count(e => e.CourseId == c.Id),
                MaxStudents = c.MaxEnrollies
            })
            .ToListAsync();

        var vm = new EnrollmentIndexVm
        {
            Enrollments = enrollments,
            AvailableCourses = availableCourses
        };

        return View(vm);
    }

    // POST: /Enrollments/Enroll
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll([FromBody] EnrollRequestVm req)
    {
        var studentIdStr = User.Claims.FirstOrDefault(c => c.Type == "StudentId")?.Value;
        if (!int.TryParse(studentIdStr, out var studentId))
            return Forbid();

        // Check if already enrolled
        var already = await _db.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == req.CourseId);
        if (already)
            return BadRequest("Already enrolled.");

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = req.CourseId,
            EnrolledOn = DateTime.UtcNow
        };
        _db.Enrollments.Add(enrollment);
        await _db.SaveChangesAsync();
        return Ok();
    }

        [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unenroll([FromBody] EnrollRequestVm req)
    {
        var studentIdStr = User.Claims.FirstOrDefault(c => c.Type == "StudentId")?.Value;
        if (!int.TryParse(studentIdStr, out var studentId))
            return Forbid();

        var enrollment = await _db.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == req.CourseId);

        if (enrollment == null)
            return BadRequest("Not enrolled in this course.");

        if (enrollment.Grade.HasValue)
            return BadRequest("Cannot unenroll from a graded course.");

        _db.Enrollments.Remove(enrollment);
        await _db.SaveChangesAsync();
        return Ok();
    }
}
