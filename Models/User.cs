using System.ComponentModel.DataAnnotations;

namespace SES.Models
{

    public enum UserRole { Student = 0, Admin = 1 }
    public class User
    {
        public int Id { get; set; }

        [Required, EmailAddress, StringLength(80)]
        public string Email { get; set; } = string.Empty;
        [Required, StringLength(40)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(40)]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public UserRole Role { get; set; } = UserRole.Student;


        //If User is a Student then this FK will be set
        public int? StudentId { get; set; }
        public Student? Student { get; set; }
    }

}