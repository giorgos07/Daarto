using Microsoft.AspNetCore.Mvc;

namespace Daarto.Models
{
    public class Search
    {
        public string Value { get; set; }

        [FromForm(Name = "regex")]
        public bool RegularExpression { get; set; }
    }
}
