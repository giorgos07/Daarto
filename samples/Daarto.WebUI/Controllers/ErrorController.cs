using Microsoft.AspNetCore.Mvc;

namespace Daarto.WebUI.Controllers
{
    public class ErrorController : Controller
    {
        public const string Name = "Error";

        public ViewResult Index(int errorCode)
        {
            return View(errorCode);
        }
    }
}