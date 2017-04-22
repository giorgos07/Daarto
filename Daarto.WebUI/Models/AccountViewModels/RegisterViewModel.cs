using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Daarto.WebUI.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your email address")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Please provide a valid email address")]
        [Remote("validate-email-address", "account")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify a password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[\W]).{6,50})",
            ErrorMessage = "Your password must be at least 6 characters long and contain at least one digit, one lowercase character and one special character")]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify your password's confirmation")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Please confirm your password correctly")]
        public string PasswordConfirmation { get; set; }
    }
}