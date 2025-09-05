using System.ComponentModel.DataAnnotations;

namespace SES.Models.ViewModels
{
    public class CourseRowVm
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int MaxStudents { get; set; }
        public string Department { get; set; } = string.Empty;
        public int EnrolledCount { get; set; }
        public bool IsFull => EnrolledCount >= MaxStudents;
    }

    public class CoursesAdminVm
    {
        public List<CourseRowVm> Courses { get; set; } = new();

        public CourseEditVm NewCourse { get; set; } = new();

        public int TotalCourses => Courses.Count;
        public int TotalEnrollments => Courses.Sum(c => c.EnrolledCount);
    }

    public class CourseEditVm
    {
        public int? Id { get; set; } // null for create

        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Department { get; set; } = string.Empty;

        [Range(1, 30)]
        public int Credits { get; set; }

        [Range(1, 500)]
        public int MaxStudents { get; set; }
    }

    // 🔹 New ViewModel: one row per student in a course
    public class StudentRowVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    // 🔹 New ViewModel: course details + enrolled students
    public class CourseStudentsVm
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public List<StudentRowVm> Students { get; set; } = new();

        public int TotalStudents => Students.Count;
    }
}
