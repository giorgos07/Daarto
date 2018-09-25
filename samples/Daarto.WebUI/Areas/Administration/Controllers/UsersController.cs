using System;
using System.Threading.Tasks;
using AspNetCore.Identity.Dapper;
using Daarto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Daarto.Controllers.Administration
{
    [Area("Administration")]
    [Authorize(Roles = "Administrator")]
    [Route("users")]
    public class UsersController : Controller
    {
        public const string Name = "Users";
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager) => _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

        [HttpGet]
        public ViewResult Index() => View();

        [HttpGet("create")]
        public ViewResult Create() => View();

        [HttpGet("{id}/edit")]
        public async Task<ViewResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            return View(user != null ? new EditUserViewModel
            {
                //LastName = user.LastName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                PhoneNumber = user.PhoneNumber,
                //FirstName = user.FirstName,
                //Address = user.Address,
                Id = user.Id
            } : null);
        }

        [HttpPost("{id}/edit")]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Response = new EditUserResponseViewModel
                {
                    Succeeded = false,
                    Description = "Request does not contain the required information to update the user."
                };

                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id.ToString());

            if (user == null)
            {
                ViewBag.Response = new EditUserResponseViewModel
                {
                    Succeeded = false,
                    Description = $"User with id: {model.Id} could not be found."
                };

                return View(model);
            }

            //user.Address = model.Address;
            user.EmailConfirmed = model.EmailConfirmed;
            //user.FirstName = model.FirstName;
            //user.LastName = model.LastName;
            user.LockoutEnabled = model.LockoutEnabled;
            user.PhoneNumber = model.PhoneNumber;
            user.LockoutEnabled = model.LockoutEnabled;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                ViewBag.Response = new EditUserResponseViewModel
                {
                    Succeeded = false,
                    Description = "The was a problem updating the user. Please try again."
                };

                return View(model);
            }

            ViewBag.Response = new EditUserResponseViewModel
            {
                Succeeded = true,
                Description = "User information were updated successfully."
            };

            return View(model);
        }
    }
}
