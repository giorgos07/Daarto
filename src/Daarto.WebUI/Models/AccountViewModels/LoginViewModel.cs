using System.ComponentModel.DataAnnotations;

namespace Daarto.WebUI.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your email address")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Please provide a valid email address")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify a password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}