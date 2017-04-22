using Daarto.IdentityProvider.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Daarto.WebUI.Infrastructure.Identity
{
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole> store, IEnumerable<IRoleValidator<ApplicationRole>> roleValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, ILogger<RoleManager<ApplicationRole>> logger, IHttpContextAccessor contextAccessor)
            : base(store, roleValidators, keyNormalizer, errors, logger, contextAccessor)
        {
        }
    }
}