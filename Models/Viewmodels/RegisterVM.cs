
using System.ComponentModel.DataAnnotations;

namespace SES.Models.ViewModels
{
    public class RegisterVm
    {
        [Required, StringLength(40)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(40)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
