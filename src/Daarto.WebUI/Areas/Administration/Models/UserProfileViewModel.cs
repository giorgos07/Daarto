using System;
using System.ComponentModel.DataAnnotations;

namespace Daarto.WebUI.Areas.Administration.Models
{
    public class UserProfileViewModel : UserViewModelBase
    {
        [Required(AllowEmptyStrings = false)]
        public Guid Id { get; set; }

        public string PhotoUrl { get; set; }
        public string PhotoName { get; set; }
    }
}