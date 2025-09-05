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

    //   public class CourseItem
    // {
    //     public int CourseId { get; set; }
    //     public string Title { get; set; } = string.Empty;
    //     public int Credits { get; set; }
    //     public float? Grade { get; set; }
    //     public string Department { get; set; } = string.Empty;
    //     public DateTime EnrolledOn { get; set; } 
    //     public int EnrolledCount { get; set; }     
    //     public int MaxStudents { get; set; }       
    // }
}