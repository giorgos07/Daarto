using Microsoft.AspNetCore.Mvc;

namespace Daarto.Models
{
    public class Column
    {
        public string Data { get; set; }
        public string Name { get; set; }

        [FromForm(Name = "searchable")]
        public bool IsSearchable { get; set; }

        [FromForm(Name = "orderable")]
        public bool IsOrderable { get; set; }
        
        public Search Search { get; set; }
    }
}
