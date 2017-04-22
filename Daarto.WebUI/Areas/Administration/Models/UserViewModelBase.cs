using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Daarto.WebUI.Areas.Administration.Models
{
    public class UserViewModelBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your email address")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Please provide a valid email address")]
        [Remote("validate-email-address", "account", "")]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public bool LockoutEnabled { get; set; }
    }
}