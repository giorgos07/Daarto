using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    internal class UserStore : IQueryableUserStore<ApplicationUser>, IUserEmailStore<ApplicationUser>, IUserLoginStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>,
        IUserPhoneNumberStore<ApplicationUser>, IUserTwoFactorStore<ApplicationUser>, IUserSecurityStampStore<ApplicationUser>,
        IUserClaimStore<ApplicationUser>, IUserLockoutStore<ApplicationUser>, IUserRoleStore<ApplicationUser>
    {
        private readonly UsersTable _usersTable;
        private readonly UsersRolesTable _usersRolesTable;
        private readonly RolesTable _rolesTable;
        private readonly UsersClaimsTable _usersClaimsTable;
        private readonly UsersLoginsTable _usersLoginsTable;

        public UserStore(IDatabaseConnectionFactory databaseConnectionFactory) {
            _usersTable = new UsersTable(databaseConnectionFactory);
            _usersRolesTable = new UsersRolesTable(databaseConnectionFactory);
            _rolesTable = new RolesTable(databaseConnectionFactory);
            _usersClaimsTable = new UsersClaimsTable(databaseConnectionFactory);
            _usersLoginsTable = new UsersLoginsTable(databaseConnectionFactory);
        }

        public IQueryable<ApplicationUser> Users => Task.Run(() => _usersTable.GetAllUsers()).Result.AsQueryable();

        #region IUserStore<ApplicationUser> Implementation
        public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return _usersTable.CreateAsync(user);
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return _usersTable.DeleteAsync(user);
        }

        public Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            var isValidGuid = Guid.TryParse(userId, out var userGuid);

            if (!isValidGuid) {
                return Task.FromResult<ApplicationUser>(null);
            }

            return _usersTable.FindByIdAsync(userGuid);
        }

        public Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            normalizedUserName.ThrowIfNull(nameof(normalizedUserName));
            return _usersTable.FindByNameAsync(normalizedUserName);
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            normalizedName.ThrowIfNull(nameof(normalizedName));
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            userName.ThrowIfNull(nameof(userName));
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return _usersTable.UpdateAsync(user);
        }

        public void Dispose() { }
        #endregion IUserStore<ApplicationUser> implementation.

        #region IUserEmailStore<ApplicationUser> implementation.
        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            email.ThrowIfNull(nameof(email));
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            normalizedEmail.ThrowIfNull(nameof(normalizedEmail));
            return _usersTable.FindByEmailAsync(normalizedEmail);
        }

        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            normalizedEmail.ThrowIfNull(nameof(normalizedEmail));
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }
        #endregion IUserEmailStore<ApplicationUser> implementation.

        #region IUserLoginStore<ApplicationUser> implementation.
        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            login.ThrowIfNull(nameof(login));
            return _usersLoginsTable.AddLoginAsync(user, login);
        }

        public Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            loginProvider.ThrowIfNull(nameof(loginProvider));
            loginProvider.ThrowIfNull(nameof(loginProvider));
            return _usersLoginsTable.RemoveLoginAsync(user, loginProvider, providerKey);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return _usersLoginsTable.GetLoginsAsync(user);
        }

        public Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            loginProvider.ThrowIfNull(nameof(loginProvider));
            return _usersLoginsTable.FindByLoginAsync(loginProvider, providerKey);
        }
        #endregion IUserLoginStore<ApplicationUser> implementation.

        #region IUserPasswordStore<ApplicationUser> implementation.
        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            passwordHash.ThrowIfNull(nameof(passwordHash));
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }
        #endregion IUserPasswordStore<ApplicationUser> implementation.

        #region IUserPhoneNumberStore<ApplicationUser> implementation.
        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }
        #endregion IUserPhoneNumberStore<ApplicationUser> implementation.

        #region IUserTwoFactorStore<ApplicationUser> implementation.
        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.TwoFactorEnabled);
        }
        #endregion IUserTwoFactorStore<ApplicationUser> implementation.

        #region IUserSecurityStampStore<ApplicationUser> implementation.
        public Task SetSecurityStampAsync(ApplicationUser user, string stamp, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            stamp.ThrowIfNull(nameof(stamp));
            user.SecurityStamp = stamp;
            return Task.FromResult<object>(null);
        }

        public Task<string> GetSecurityStampAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.SecurityStamp);
        }
        #endregion IUserSecurityStampStore<ApplicationUser> implementation.

        #region IUserClaimStore<ApplicationUser> implementation.
        public Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return _usersClaimsTable.GetClaimsAsync(user);
        }

        public Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            claims.ThrowIfNull(nameof(claims));
            return _usersClaimsTable.AddClaimsAsync(user, claims);
        }

        public Task ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            claim.ThrowIfNull(nameof(claim));
            newClaim.ThrowIfNull(nameof(newClaim));
            return _usersClaimsTable.ReplaceClaimAsync(user, claim, newClaim);
        }

        public Task RemoveClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
        #endregion IUserClaimStore<ApplicationUser> 

        #region IUserLockoutStore<ApplicationUser> implementation.
        public Task<DateTimeOffset?> GetLockoutEndDateAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.LockoutEndDateTimeUtc.HasValue ? new DateTimeOffset?(DateTime.SpecifyKind(user.LockoutEndDateTimeUtc.Value, DateTimeKind.Utc)) : null);
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            user.LockoutEndDateTimeUtc = lockoutEnd?.UtcDateTime;
            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }
        #endregion IUserLockoutStore<ApplicationUser> implementation.

        #region IUserRoleStore<ApplicationUser> implementation.
        public Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            roleName.ThrowIfNull(nameof(roleName));
            var role = Task.Run(() => _rolesTable.GetAllRoles(), cancellationToken).Result.SingleOrDefault(e => e.NormalizedName == roleName);
            return _usersRolesTable.AddToRoleAsync(user, role.Id);
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            var role = Task.Run(() => _rolesTable.GetAllRoles(), cancellationToken).Result.SingleOrDefault(e => e.NormalizedName == roleName);
            return _usersRolesTable.RemoveFromRoleAsync(user, role.Id);
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            return _usersRolesTable.GetRolesAsync(user);
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            user.ThrowIfNull(nameof(user));
            roleName.ThrowIfNull(nameof(roleName));
            var userRoles = await GetRolesAsync(user, cancellationToken);
            return userRoles.Contains(roleName);
        }

        public Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
        #endregion IUserRoleStore<ApplicationUser> implementation.
    }
}