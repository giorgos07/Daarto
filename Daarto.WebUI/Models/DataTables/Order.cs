using Microsoft.AspNetCore.Mvc;

namespace Daarto.WebUI.Models.DataTables
{
    public class Order
    {
        [FromForm(Name = "dir")]
        public string Direction { get; set; }
        
        public int Column { get; set; }
    }
}