using AspNetCore.Identity.Dapper;
using Daarto.WebUI.Infrastructure.Services;
using Daarto.WebUI.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Daarto.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHostingEnvironment hostingEnvironment, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hostingEnvironment = hostingEnvironment;
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();

            // If the user is already authenticated we do not need to display the login page, so we redirect to the landing page.
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index", "home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Response = new LoginResponseViewModel
                {
                    Succeeded = false,
                    Description = "Request does not contain the required information to log the user in."
                };

                return View();
            }

            ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    ViewBag.Response = new LoginResponseViewModel
                    {
                        Succeeded = false,
                        Description = "Please confirm your account before you try to log in."
                    };

                    return View();
                }

                if (user.LockoutEnabled)
                {
                    ViewBag.Response = new LoginResponseViewModel
                    {
                        Succeeded = false,
                        Description = "Your account is locked. Please contact support."
                    };

                    return View();
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);

                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
            }

            ViewBag.Response = new LoginResponseViewModel
            {
                Succeeded = false,
                Description = "Please check your credentials"
            };

            return View();
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            // If the user is already authenticated we do not need to display the registration page, so we redirect to the landing page.
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index", "home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Response = new RegisterResponseViewModel
                {
                    Succeeded = false,
                    Description = "Request does not contain the required information to register the user."
                };

                return View();
            }

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = model.Email,
                Email = model.Email,
                RegistrationDate = DateTime.Now
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                ViewBag.Response = new RegisterResponseViewModel
                {
                    Succeeded = false,
                    Description = $"The account {model.Email} could not be created. Please try again."
                };

                return View();
            }

            await _userManager.AddToRoleAsync(user, "User");
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string callbackUrl = Url.Action("confirm-email", "account", new
            {
                userId = user.Id,
                code
            }, HttpContext.Request.Scheme);

            if (!_hostingEnvironment.IsDevelopment())
            {
                await _emailSender.SendEmailAsync(model.Email, "Confirm your account", $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>.");
            }
            else
            {
                // If we are working locally, there is no need to send an actual email.
                Debug.WriteLine($"Use the following url to confirm your account locally: {callbackUrl}");
            }

            ViewBag.Response = new RegisterResponseViewModel
            {
                Succeeded = true,
                Description = $"User account was created successfully. A message has been sent to {user.Email} in order to confirm your email address."
            };

            return View();
        }

        [HttpGet]
        [ActionName("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("index", "error", new { errorCode = "500" });
            }

            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("index", "error", new { errorCode = "500" });
            }

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);

            if (!result.Succeeded)
            {
                return RedirectToAction("index", "error", new { errorCode = "500" });
            }

            await _userManager.SetLockoutEnabledAsync(user, false);
            return View("confirm-email");
        }

        [Authorize]
        [HttpPost]
        [ActionName("log-out")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        [ActionName("validate-email-address")]
        public async Task<JsonResult> ValidateEmailAddress(string email)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            return user != null ? Json($"Email {email} is already in use") : Json(true);
        }

        [HttpGet]
        [ActionName("forgot-password")]
        public ViewResult ForgotPassword() => View();

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("index", "home");
        }
    }
}