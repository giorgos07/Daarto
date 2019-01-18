using System;
using System.ComponentModel.DataAnnotations;

namespace Daarto.Models
{
    public class EditUserViewModel : UserViewModelBase
    {
        [Required(AllowEmptyStrings = false)]
        public Guid Id { get; set; }
    }
}