using Daarto.IdentityProvider.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Daarto.WebUI.Infrastructure.Identity
{
    public class ApplicationSignInManager : SignInManager<ApplicationUser>
    {
        public ApplicationSignInManager(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<ApplicationUser>> logger)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger)
        {
        }
    }
}