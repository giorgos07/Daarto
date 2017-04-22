using Microsoft.AspNetCore.Mvc;

namespace Daarto.WebUI.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ViewResult Index()
        {
            return View();
        }
    }
}