using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Identity.Dapper.Tests
{
    public class UserManagerSpecificationTests : UserManagerSpecificationTestBase<IdentityUser>
    {
        protected override void AddUserStore(IServiceCollection services, object context = null) {
            throw new NotImplementedException();
        }

        protected override object CreateTestContext() {
            throw new NotImplementedException();
        }

        protected override IdentityUser CreateTestUser(string namePrefix = "", string email = "", string phoneNumber = "", bool lockoutEnabled = false, DateTimeOffset? lockoutEnd = null, bool useNamePrefixAsUserName = false) {
            throw new NotImplementedException();
        }

        protected override void SetUserPasswordHash(IdentityUser user, string hashedPassword) {
            throw new NotImplementedException();
        }

        protected override Expression<Func<IdentityUser, bool>> UserNameEqualsPredicate(string userName) {
            throw new NotImplementedException();
        }

        protected override Expression<Func<IdentityUser, bool>> UserNameStartsWithPredicate(string userName) {
            throw new NotImplementedException();
        }
    }
}
