// Models/Course.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SES.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required, StringLength(80)]
        public string Title { get; set; } = string.Empty;

        [Range(0, 60)]
        public int Credits { get; set; }

        [StringLength(20)]
        public string? Department { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
