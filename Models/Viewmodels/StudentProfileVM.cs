// Models/ViewModels/StudentProfileVm.cs
namespace SES.Models.ViewModels
{
    public class StudentProfileVm
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName  { get; set; } = string.Empty;
        public DateTime Since  { get; set; }

        public List<CourseItem> Courses { get; set; } = new();
    }

    public class CourseItem
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Credits { get; set; }
        public float? Grade { get; set; }
        public DateTime EnrolledOn { get; set; } 
        public int EnrolledCount { get; set; }     
        public int MaxStudents { get; set; }       
    }
}
