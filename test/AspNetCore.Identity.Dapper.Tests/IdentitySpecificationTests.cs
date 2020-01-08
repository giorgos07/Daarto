using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Identity.Dapper.Tests
{
    public class IdentitySpecificationTests : IdentitySpecificationTestBase<IdentityUser, IdentityRole, string>
    {
        protected override void AddRoleStore(IServiceCollection services, object context = null) {
            throw new NotImplementedException();
        }

        protected override void AddUserStore(IServiceCollection services, object context = null) {
            throw new NotImplementedException();
        }

        protected override object CreateTestContext() {
            throw new NotImplementedException();
        }

        protected override IdentityRole CreateTestRole(string roleNamePrefix = "", bool useRoleNamePrefixAsRoleName = false) {
            return new IdentityRole {
                Name = useRoleNamePrefixAsRoleName ? roleNamePrefix : $"{roleNamePrefix}{Guid.NewGuid()}"
            };
        }

        protected override IdentityUser CreateTestUser(string namePrefix = "", string email = "", string phoneNumber = "", bool lockoutEnabled = false, DateTimeOffset? lockoutEnd = null, bool useNamePrefixAsUserName = false) {
            return new IdentityUser {
                UserName = useNamePrefixAsUserName ? namePrefix : $"{namePrefix}{Guid.NewGuid()}",
                Email = email,
                PhoneNumber = phoneNumber,
                LockoutEnabled = lockoutEnabled,
                LockoutEnd = lockoutEnd
            };
        }

        protected override Expression<Func<IdentityRole, bool>> RoleNameEqualsPredicate(string roleName) {
            return role => role.Name == roleName;
        }

        protected override Expression<Func<IdentityRole, bool>> RoleNameStartsWithPredicate(string roleName) {
            return role => role != null && role.Name.StartsWith(roleName);
        }

        protected override void SetUserPasswordHash(IdentityUser user, string hashedPassword) {
            user.PasswordHash = hashedPassword;
        }

        protected override Expression<Func<IdentityUser, bool>> UserNameEqualsPredicate(string userName) {
            return user => user.UserName == userName;
        }

        protected override Expression<Func<IdentityUser, bool>> UserNameStartsWithPredicate(string userName) {
            return user => user != null && user.UserName.StartsWith(userName);
        }
    }
}
