using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Daarto.Infrastructure.Filters;
using Daarto.Infrastructure.Settings;
using Daarto.Models;
using Daarto.Security;
using Identity.Dapper.Postgres.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Daarto.Controllers.Administration
{
    [Area("Administration")]
    [Authorize(Roles = "Administrator")]
    [Route("profile")]
    public class ProfileController : Controller
    {
        public const string Name = "Profile";
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppSettings _appSettings;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfileController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings, SignInManager<ApplicationUser> signInManager) {
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _signInManager = signInManager;
        }

        [HttpGet("edit")]
        [GenerateAntiforgeryTokenCookieForAjax]
        public async Task<ViewResult> Edit() {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var photoName = User.GetClaimValue(ClaimTypes.Uri);

            return View(new EditProfileRequest {
                LastName = User.GetClaimValue(ClaimTypes.Surname),
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                PhoneNumber = user.PhoneNumber,
                FirstName = User.GetClaimValue(ClaimTypes.GivenName),
                Address = User.GetClaimValue(ClaimTypes.StreetAddress),
                Id = user.Id,
                PhotoName = photoName,
                PhotoUrl = !string.IsNullOrEmpty(photoName) 
                    ? $"{_appSettings.Domain}/{_appSettings.UploadsFolder}/{user.Id}/{photoName}" 
                    : $"{_appSettings.Domain}/img/default_profile.png"
            });
        }

        [HttpPost("edit")]
        public async Task<IActionResult> Edit(EditProfileRequest model) {
            if (!ModelState.IsValid) {
                ViewBag.Response = new EditProfileResult {
                    Succeeded = false,
                    Description = "Request does not contain the required information to update your profile."
                };

                return View(model);
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            var claims = new List<Claim>();
            if (!string.IsNullOrEmpty(model.PhotoName)) { claims.Add(new Claim(ClaimTypes.Uri, model.PhotoName)); }
            if (!string.IsNullOrEmpty(model.Address)) { claims.Add(new Claim(ClaimTypes.StreetAddress, model.Address)); }
            if (!string.IsNullOrEmpty(model.FirstName)) { claims.Add(new Claim(ClaimTypes.GivenName, model.FirstName)); }
            if (!string.IsNullOrEmpty(model.LastName)) { claims.Add(new Claim(ClaimTypes.Surname, model.LastName)); }
            user.PhoneNumber = model.PhoneNumber;
            // Built-in UserManager internally calls UpdateAsync so no need to call it explicitly.
            // https://github.com/aspnet/Identity/blob/master/src/Core/UserManager.cs#L1044
            var result = await _userManager.AddClaimsAsync(user, claims);

            if (!result.Succeeded) {
                ViewBag.Response = new EditProfileResult {
                    Succeeded = false,
                    Description = "The was a problem updating your profile. Please try again."
                };

                return View(model);
            }

            // We need to sign out and re-sign in the user in the background in case he changed his photo url. Photo url exists
            // in a user claim. Claims are updated when the sign in cookie is generated.
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(user, true);

            ViewBag.Response = new EditProfileResult {
                Succeeded = true,
                Description = "Your profile was updated successfully."
            };

            model.PhotoUrl = !string.IsNullOrEmpty(model.PhotoName) 
                ? $"{_appSettings.Domain}/{_appSettings.UploadsFolder}/{user.Id}/{model.PhotoName}" 
                : $"{_appSettings.Domain}/img/default_profile.png";

            return View(model);
        }
    }
}
