using Microsoft.AspNetCore.Mvc;

namespace Daarto.WebUI.Controllers
{
    public class HomeController : Controller
    {
        public const string Name = "Home";

        [HttpGet]
        public ViewResult Index()
        {
            return View();
        }
    }
}
