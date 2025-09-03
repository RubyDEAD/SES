using SES.Models.ViewModels;
namespace SES.Models.ViewModels
{
    public class EnrollmentIndexVm
    {
        public IEnumerable<SES.Models.Enrollment> Enrollments { get; set; } = new List<SES.Models.Enrollment>();
        public IEnumerable<CourseItem> AvailableCourses { get; set; } = new List<CourseItem>();
    }
    public class EnrollRequestVm
    {
        public int CourseId { get; set; }
    }
}