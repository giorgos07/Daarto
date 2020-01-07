using Microsoft.AspNetCore.Identity;

namespace Daarto.WebUI.Data
{
    public class ExtendedIdentityRole : IdentityRole<string>
    {
        public ExtendedIdentityRole() { }

        public string Description { get; set; }
    }
}
