using System;
using System.ComponentModel.DataAnnotations;

namespace Daarto.Models
{
    public class EditProfileRequest : UserViewModelBase
    {
        [Required(AllowEmptyStrings = false)]
        public string Id { get; set; }

        public string PhotoUrl { get; set; }
        public string PhotoName { get; set; }
    }
}