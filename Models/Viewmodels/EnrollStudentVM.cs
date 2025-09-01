using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SES.Models.ViewModels
{
    public class EnrollStudentVm
    {
        [Required]
        public int StudentId { get; set; }
        [Required]
        public int CourseId { get; set; }

        public IEnumerable<SelectListItem> Students { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Courses { get; set; } = new List<SelectListItem>();
    }
}
