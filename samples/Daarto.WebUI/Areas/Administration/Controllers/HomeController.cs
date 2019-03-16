using Daarto.Abstractions;
using Daarto.Infrastructure.Caching;
using Daarto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Daarto.Controllers.Administration
{
    [Area("Administration")]
    [Authorize(Roles = "Administrator")]
    public class HomeController : Controller
    {
        public const string Name = "Home";
        private readonly IUserRepository _userRepository;
        private readonly ICacheManagerService _cacheManagerService;

        public HomeController(IUserRepository userRepository, ICacheManagerService cacheManagerService) {
            _userRepository = userRepository;
            _cacheManagerService = cacheManagerService;
        }

        [HttpGet]
        public ViewResult Index() {
            //_cacheManagerService.TryGetAndSet(CacheKeys.TotalNumberOfUsers, () => _userRepository.GetTotalNumberOfUsers(), out int totalNumberOfUsers);

            return View(new HomePageViewModel {
                TotalNumberOfUsers = 9
            });
        }
    }
}
