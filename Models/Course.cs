// Models/Course.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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


        [Range(1, 100)]
        public int MaxEnrollies { get; set;} = 20;

        [NotMapped]
        public int CurrentEnrollies => Enrollments?.Count ?? 0;

    }
}
