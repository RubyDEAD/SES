// Models/Student.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SES.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required, StringLength(40)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(40)]
        public string LastName { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
