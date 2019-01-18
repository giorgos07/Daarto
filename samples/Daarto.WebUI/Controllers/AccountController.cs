using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Daarto.Abstractions;
using Daarto.Models;
using Identity.Dapper.Postgres.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Daarto.WebUI.Controllers
{
    public class AccountController : Controller
    {
        public const string Name = "Account";
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHostingEnvironment hostingEnvironment, IEmailService emailService) {
            _userManager = userManager;
            _signInManager = signInManager;
            _hostingEnvironment = hostingEnvironment;
            _emailService = emailService;
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login(string returnUrl = null) {
            await _signInManager.SignOutAsync();

            // If the user is already authenticated we do not need to display the login page, so we redirect to the landing page.
            if (User.Identity.IsAuthenticated) {
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null) {
            if (!ModelState.IsValid) {
                ViewBag.Response = new LoginResponseViewModel {
                    Succeeded = false,
                    Description = "Request does not contain the required information to log the user in."
                };

                return View();
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null) {
                if (!user.EmailConfirmed) {
                    ViewBag.Response = new LoginResponseViewModel {
                        Succeeded = false,
                        Description = "Please confirm your account before you try to log in."
                    };

                    return View();
                }

                if (user.LockoutEnabled) {
                    ViewBag.Response = new LoginResponseViewModel {
                        Succeeded = false,
                        Description = "Your account is locked. Please contact support."
                    };

                    return View();
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);

                if (result.Succeeded) {
                    return RedirectToLocal(returnUrl);
                }
            }

            ViewBag.Response = new LoginResponseViewModel {
                Succeeded = false,
                Description = "Please check your credentials"
            };

            return View();
        }

        [HttpGet("register")]
        public IActionResult Register(string returnUrl = null) {
            // If the user is already authenticated we do not need to display the registration page, so we redirect to the landing page.
            if (User.Identity.IsAuthenticated) {
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null) {
            if (!ModelState.IsValid) {
                ViewBag.Response = new RegisterResponseViewModel {
                    Succeeded = false,
                    Description = "Request does not contain the required information to register the user."
                };

                return View();
            }

            var user = new ApplicationUser {
                Id = $"{Guid.NewGuid()}",
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) {
                ViewBag.Response = new RegisterResponseViewModel {
                    Succeeded = false,
                    Description = $"The account {model.Email} could not be created. Please try again."
                };

                return View();
            }

            await _userManager.AddToRoleAsync(user, "User");
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var callbackUrl = Url.Action(nameof(ConfirmEmail), Name, new {
                userId = user.Id,
                code
            }, HttpContext.Request.Scheme);

            if (!_hostingEnvironment.IsDevelopment()) {
                await _emailService.SendAsync(new List<EmailRecipient> {
                    new EmailRecipient {
                        EmailAddress = model.Email
                    }
                }, "Confirm your account", $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>.");
            } else {
                // If we are working locally, there is no need to send an actual email.
                Debug.WriteLine($"Use the following url to confirm your account locally: {callbackUrl}");
            }

            ViewBag.Response = new RegisterResponseViewModel {
                Succeeded = true,
                Description = $"User account was created successfully. A message has been sent to {user.Email} in order to confirm your email address."
            };

            return View();
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code) {
            if (userId == null || code == null) {
                return RedirectToAction("index", "error", new { errorCode = "500" });
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) {
                return RedirectToAction("index", "error", new { errorCode = "500" });
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (!result.Succeeded) {
                return RedirectToAction(nameof(ErrorController.Index), ErrorController.Name, new { errorCode = "500" });
            }

            await _userManager.SetLockoutEnabledAsync(user, false);
            return View(nameof(ConfirmEmail));
        }

        [Authorize]
        [HttpPost("log-out")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut() {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
        }

        [HttpGet("validate-email-address")]
        public async Task<JsonResult> ValidateEmailAddress(string email) {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null ? Json($"Email {email} is already in use") : Json(true);
        }

        [HttpGet("forgot-password")]
        public ViewResult ForgotPassword() => View();

        private IActionResult RedirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            }

            return RedirectToAction("index", "home");
        }
    }
}
