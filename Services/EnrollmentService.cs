using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SES.Data;
using SES.Models;

namespace SES.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly SchoolContext _db;
        public EnrollmentService(SchoolContext db) => _db = db;

        // Enroll any student into a course (unrestricted)
        public async Task<(bool ok, string? error)> EnrollAsync(int studentId, int courseId)
        {
            var exists = await _db.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
            if (exists) return (false, "Student already enrolled in this course.");

            var student = await _db.Students.FindAsync(studentId);
            var course = await _db.Courses.FindAsync(courseId);
            if (student == null || course == null) return (false, "Invalid Student or Course.");

            _db.Enrollments.Add(new Enrollment { StudentId = studentId, CourseId = courseId });
            await _db.SaveChangesAsync();
            return (true, null);
        }

        // Enroll with user authorization: Admin can enroll any student; Student can only enroll self
        public async Task<(bool ok, string? error)> EnrollAsync(int actingUserId, int studentId, int courseId)
        {
            var actor = await _db.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == actingUserId);
            if (actor == null) return (false, "Unauthorized.");

            var isAdmin = actor.Role == UserRole.Admin;
            var isSelf = actor.Role == UserRole.Student && actor.StudentId.HasValue && actor.StudentId.Value == studentId;

            if (!(isAdmin || isSelf))
                return (false, "Insufficient privileges.");

            return await EnrollAsync(studentId, courseId);
        }

        // Grade enrollment (unrestricted)
        public async Task<(bool ok, string? error)> SetGradeAsync(int enrollmentId, float grade)
        {
            if (grade < 0f || grade > 4f) return (false, "Grade must be between 0 and 4.");

            var enr = await _db.Enrollments.FindAsync(enrollmentId);
            if (enr == null) return (false, "Enrollment not found.");

            enr.Grade = grade;
            await _db.SaveChangesAsync();
            return (true, null);
        }

        // Grade with user authorization: Admin can grade any; Student can only grade their own enrollment (often disallowed)
        public async Task<(bool ok, string? error)> SetGradeAsync(int actingUserId, int enrollmentId, float grade)
        {
            var actor = await _db.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == actingUserId);
            if (actor == null) return (false, "Unauthorized.");

            var enr = await _db.Enrollments.AsNoTracking().SingleOrDefaultAsync(e => e.Id == enrollmentId);
            if (enr == null) return (false, "Enrollment not found.");

            var isAdmin = actor.Role == UserRole.Admin;
            var isOwnEnrollment = actor.Role == UserRole.Student && actor.StudentId.HasValue && actor.StudentId.Value == enr.StudentId;

            // Typical policy: only Admins may set grades; block students from grading themselves.
            if (!isAdmin)
                return (false, "Only admins can set grades.");

            // If policy allows students to grade self, replace with:
            // if (!(isAdmin || isOwnEnrollment)) return (false, "Insufficient privileges.");

            return await SetGradeAsync(enrollmentId, grade);
        }
    }
}
