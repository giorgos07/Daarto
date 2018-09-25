using System.ComponentModel.DataAnnotations;

namespace Daarto.Models
{
    public class EditUserViewModel : UserViewModelBase
    {
        [Required(AllowEmptyStrings = false)]
        public string Id { get; set; }
    }
}