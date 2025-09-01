// Models/Enrollment.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace SES.Models
{
   

    public class Enrollment
    {
        public int Id { get; set; }

        // FKs
        public int StudentId { get; set; }
        public int CourseId { get; set; }

        // Navs
        public Student Student { get; set; } = default!;
        public Course Course { get; set; } = default!;

        public float? Grade { get; set; }

        [DataType(DataType.Date)]
        public DateTime EnrolledOn { get; set; } = DateTime.UtcNow;
    }
}
