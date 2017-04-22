using Microsoft.AspNetCore.Mvc;

namespace Daarto.WebUI.Controllers
{
    public class ErrorController : Controller
    {
        public ViewResult Index(int errorCode)
        {
            return View(errorCode);
        }
    }
}