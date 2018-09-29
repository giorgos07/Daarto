using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Identity.Dapper;
using Daarto.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Daarto.Infrastructure.Identity
{
    public class UserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptions<IdentityOptions> optionsAccessor,
            IOptions<AppSettings> appSettings) : base(userManager, roleManager, optionsAccessor) {

            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user) {
            var principal = await base.CreateAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);
            ((ClaimsIdentity)principal.Identity).AddClaims(claims);
            return principal;
        }
    }
}
