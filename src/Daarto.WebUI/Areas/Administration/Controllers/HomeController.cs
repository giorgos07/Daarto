using Daarto.DataAccess.Abstract;
using Daarto.Services.Abstract;
using Daarto.WebUI.Areas.Administration.Models;
using Daarto.WebUI.Infrastructure.Caching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Daarto.WebUI.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize(Roles = "Administrator")]
    public class HomeController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheManagerService _cacheManagerService;

        public HomeController(IUserRepository userRepository, ICacheManagerService cacheManagerService)
        {
            _userRepository = userRepository;
            _cacheManagerService = cacheManagerService;
        }

        [HttpGet]
        public ViewResult Index()
        {
            _cacheManagerService.TryGetAndSet(CacheKeys.TotalNumberOfUsers, () => _userRepository.GetTotalNumberOfUsers(), out int totalNumberOfUsers);

            return View(new HomePageViewModel
            {
                TotalNumberOfUsers = totalNumberOfUsers
            });
        }
    }
}