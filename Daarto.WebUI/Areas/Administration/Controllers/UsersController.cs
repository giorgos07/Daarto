using Daarto.DataAccess.Abstract;
using Daarto.DataAccess.Models;
using Daarto.IdentityProvider.Entities;
using Daarto.WebUI.Areas.Administration.Models;
using Daarto.WebUI.Models.DataTables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Daarto.WebUI.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;

        public UsersController(UserManager<ApplicationUser> userManager, IUserRepository userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        [HttpGet]
        [ActionName("get-all")]
        public ViewResult GetAllUsers()
        {
            return View();
        }

        [HttpPost]
        [ActionName("get-all-json")]
        public async Task<JsonResult> GetAllUsersJson(DataTable dataTable)
        {
            int pageNumber = dataTable.Start / dataTable.Length + 1;
            Order order = dataTable.Order.FirstOrDefault();

            SortDirection sortDirection = order != null
                ? (order.Direction == "asc" ? SortDirection.Ascending : SortDirection.Descending)
                : SortDirection.Ascending;

            User[] users = (await _userRepository.GetUsersAsync(pageNumber, dataTable.Length, order?.Column ?? 0, sortDirection,
                string.IsNullOrEmpty(dataTable.Search.Value) ? string.Empty : dataTable.Search.Value)).ToArray();

            int totalNumberOfUsers = _userRepository.GetTotalNumberOfUsers();

            return Json(new DataTableResponse<User>
            {
                Data = users,
                RecordsFiltered = string.IsNullOrEmpty(dataTable.Search.Value) ? totalNumberOfUsers : users.Length,
                Draw = dataTable.Draw,
                RecordsTotal = totalNumberOfUsers
            });
        }

        [HttpGet]
        [ActionName("edit")]
        public async Task<ViewResult> EditUser(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            return View(user != null ? new EditUserViewModel
            {
                LastName = user.LastName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                Address = user.Address,
                Id = user.Id
            } : null);
        }
    }
}