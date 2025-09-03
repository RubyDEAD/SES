using System.ComponentModel.DataAnnotations;

namespace SES.Models.ViewModels
{
    public class CourseRowVm
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int MaxStudents { get; set; }
        public int EnrolledCount { get; set; }
        public bool IsFull => EnrolledCount >= MaxStudents;
    }

    public class CoursesAdminVm
    {
        public List<CourseRowVm> Courses { get; set; } = new();

        // Used by the inline create form in AdminIndex.cshtml
        public CourseEditVm NewCourse { get; set; } = new();

        public int TotalCourses => Courses.Count;
        public int TotalEnrollments => Courses.Sum(c => c.EnrolledCount);
    }

    public class CourseEditVm
    {
        public int? Id { get; set; } // null for create

        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Range(1, 30)]
        public int Credits { get; set; }

        [Range(1, 500)]
        public int MaxStudents { get; set; }
    }
}
