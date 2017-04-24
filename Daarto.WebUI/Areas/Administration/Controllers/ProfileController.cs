using System.Threading.Tasks;
using Daarto.IdentityProvider.Entities;
using Daarto.WebUI.Areas.Administration.Models;
using Daarto.WebUI.Infrastructure.Filters;
using Daarto.WebUI.Infrastructure.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Daarto.WebUI.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize(Roles = "Administrator")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppSettings _appSettings;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfileController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _signInManager = signInManager;
        }

        [HttpGet]
        [ActionName("edit")]
        [GenerateAntiforgeryTokenCookieForAjax]
        public async Task<ViewResult> Edit()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            return View(user != null ? new UserProfileViewModel
            {
                LastName = user.LastName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                Address = user.Address,
                Id = user.Id,
                PhotoName = user.PhotoUrl,
                PhotoUrl = !string.IsNullOrEmpty(user.PhotoUrl)
                    ? $"{_appSettings.Domain}/{_appSettings.UploadsFolder}/{user.Id}/{user.PhotoUrl}"
                    : $"{_appSettings.Domain}/img/default_profile.png"
            } : null);
        }

        [HttpPost]
        [ActionName("edit")]
        public async Task<IActionResult> Edit(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Response = new EditUserProfileResponseViewModel
                {
                    Succeeded = false,
                    Description = "Request does not contain the required information to update your profile."
                };

                return View(model);
            }

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            user.PhotoUrl = model.PhotoName;
            user.Address = model.Address;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            IdentityResult result = await _userManager.UpdateAsync(user);

            model.PhotoUrl = !string.IsNullOrEmpty(user.PhotoUrl)
                ? $"{_appSettings.Domain}/{_appSettings.UploadsFolder}/{user.Id}/{user.PhotoUrl}"
                : $"{_appSettings.Domain}/img/default_profile.png";

            if (!result.Succeeded)
            {
                ViewBag.Response = new EditUserProfileResponseViewModel
                {
                    Succeeded = false,
                    Description = "The was a problem updating your profile. Please try again."
                };

                return View(model);
            }

            // We need to sign out and re-sign in the user in the background in case he changed his photo url. Photo url exists
            // in a user claim. Claims are updated when the sign in cookie is generated.
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(user, true);

            ViewBag.Response = new EditUserProfileResponseViewModel
            {
                Succeeded = true,
                Description = "Your profile was updated successfully."
            };

            return View(model);
        }
    }
}