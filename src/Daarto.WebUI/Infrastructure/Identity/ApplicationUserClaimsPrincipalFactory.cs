using AspNetCore.Identity.Dapper;
using Daarto.WebUI.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Daarto.WebUI.Infrastructure.Identity
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        private readonly AppSettings _appSettings;

        public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor, IOptions<AppSettings> appSettings)
            : base(userManager, roleManager, optionsAccessor)
        {
            _appSettings = appSettings.Value;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            ClaimsPrincipal principal = await base.CreateAsync(user);

            ((ClaimsIdentity)principal.Identity).AddClaims(new[]
            {
                new Claim(ClaimTypes.Uri, !string.IsNullOrEmpty(user.PhotoUrl)
                    ? $"{_appSettings.Domain}/{_appSettings.UploadsFolder}/{user.Id}/{user.PhotoUrl}"
                    : $"{_appSettings.Domain}/img/default_profile.png")
            });

            return principal;
        }
    }
}
