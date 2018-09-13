using Daarto.IdentityProvider.Entities;
using Daarto.IdentityProvider.Tables;
using Daarto.Services.Abstract;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Daarto.IdentityProvider.Stores
{
    public class UserStore : IQueryableUserStore<ApplicationUser>, IUserEmailStore<ApplicationUser>, IUserLoginStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>,
        IUserPhoneNumberStore<ApplicationUser>, IUserTwoFactorStore<ApplicationUser>, IUserSecurityStampStore<ApplicationUser>,
        IUserClaimStore<ApplicationUser>, IUserLockoutStore<ApplicationUser>, IUserRoleStore<ApplicationUser>
    {
        private readonly UsersTable _usersTable;
        private readonly UsersRolesTable _usersRolesTable;
        private readonly RolesTable _rolesTable;
        private readonly UsersClaimsTable _usersClaimsTable;
        private readonly UsersLoginsTable _usersLoginsTable;

        public UserStore(IDatabaseConnectionService databaseConnection)
        {
            SqlConnection sqlConnection = databaseConnection.CreateConnection();
            _usersTable = new UsersTable(sqlConnection);
            _usersRolesTable = new UsersRolesTable(sqlConnection);
            _rolesTable = new RolesTable(sqlConnection);
            _usersClaimsTable = new UsersClaimsTable(sqlConnection);
            _usersLoginsTable = new UsersLoginsTable(sqlConnection);
        }

        public IQueryable<ApplicationUser> Users => Task.Run(() => _usersTable.GetAllUsers()).Result.AsQueryable();

        #region IUserStore<ApplicationUser> implementation.
        public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            return _usersTable.CreateAsync(user, cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            return _usersTable.DeleteAsync(user, cancellationToken);
        }

        public Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            bool isValidGuid = Guid.TryParse(userId, out Guid userGuid);

            if (!isValidGuid)
            {
                return Task.FromResult<ApplicationUser>(null);
            }

            cancellationToken.ThrowIfCancellationRequested();
            return _usersTable.FindByIdAsync(userGuid);
        }

        public Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(normalizedUserName))
            {
                throw new ArgumentNullException(nameof(normalizedUserName), "Parameter normalizedUserName cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return _usersTable.FindByNameAsync(normalizedUserName);
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(normalizedName))
            {
                throw new ArgumentNullException(nameof(normalizedName), "Parameter normalizedName cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.NormalizedUserName = normalizedName;
            return Task.FromResult<object>(null);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName), "Parameter userName cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.UserName = userName;
            return Task.FromResult<object>(null);
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            return _usersTable.UpdateAsync(user, cancellationToken);
        }

        public void Dispose()
        {
            _usersTable.Dispose();
        }
        #endregion IUserStore<ApplicationUser> implementation.

        #region IUserEmailStore<ApplicationUser> implementation.
        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email), "Parameter email cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.Email = email;
            return Task.FromResult<object>(null);
        }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.EmailConfirmed = confirmed;
            return Task.FromResult<object>(null);
        }

        public Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(normalizedEmail))
            {
                throw new ArgumentNullException(nameof(normalizedEmail), "Parameter normalizedEmail cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return _usersTable.FindByEmailAsync(normalizedEmail);
        }

        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(normalizedEmail))
            {
                throw new ArgumentNullException(nameof(normalizedEmail), "Parameter normalizedEmail cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.NormalizedEmail = normalizedEmail;
            return Task.FromResult<object>(null);
        }
        #endregion IUserEmailStore<ApplicationUser> implementation.

        #region IUserLoginStore<ApplicationUser> implementation.
        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            if (login == null)
            {
                throw new ArgumentNullException(nameof(login), "Parameter login is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return _usersLoginsTable.AddLoginAsync(user, login);
        }

        public Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(loginProvider))
            {
                throw new ArgumentNullException(nameof(loginProvider), "Parameter loginProvider and providerKey cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(providerKey))
            {
                throw new ArgumentNullException(nameof(providerKey), "Parameter providerKey and providerKey cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return _usersLoginsTable.RemoveLoginAsync(user, loginProvider, providerKey);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return _usersLoginsTable.GetLoginsAsync(user, cancellationToken);
        }

        public Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(loginProvider))
            {
                throw new ArgumentNullException(nameof(loginProvider), "Parameter loginProvider cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(providerKey))
            {
                throw new ArgumentNullException(nameof(providerKey), "Parameter providerKey cannot be null or empty.");
            }

            return _usersLoginsTable.FindByLoginAsync(loginProvider, providerKey, cancellationToken);
        }
        #endregion IUserLoginStore<ApplicationUser> implementation.

        #region IUserPasswordStore<ApplicationUser> implementation.
        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(passwordHash))
            {
                throw new ArgumentNullException(nameof(passwordHash), "Parameter passwordHash cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.PasswordHash = passwordHash;
            return Task.FromResult<object>(null);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }
        #endregion IUserPasswordStore<ApplicationUser> implementation.

        #region IUserPhoneNumberStore<ApplicationUser> implementation.
        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.PhoneNumber = phoneNumber;
            return Task.FromResult<object>(null);
        }

        public Task<string> GetPhoneNumberAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult<object>(null);
        }
        #endregion IUserPhoneNumberStore<ApplicationUser> implementation.

        #region IUserTwoFactorStore<ApplicationUser> implementation.
        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.TwoFactorEnabled = enabled;
            return Task.FromResult<object>(null);
        }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.TwoFactorEnabled);
        }
        #endregion IUserTwoFactorStore<ApplicationUser> implementation.

        #region IUserSecurityStampStore<ApplicationUser> implementation.
        public Task SetSecurityStampAsync(ApplicationUser user, string stamp, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.SecurityStamp = stamp;
            return Task.FromResult<object>(null);
        }

        public Task<string> GetSecurityStampAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.SecurityStamp);
        }
        #endregion IUserSecurityStampStore<ApplicationUser> implementation.

        #region IUserClaimStore<ApplicationUser> implementation.
        public Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            return _usersClaimsTable.GetClaimsAsync(user, cancellationToken);
        }

        public Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims), "Parameter claims is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return _usersClaimsTable.AddClaimsAsync(user, claims);
        }

        public Task ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim), "Parameter claim is not set to an instance of an object.");
            }

            if (newClaim == null)
            {
                throw new ArgumentNullException(nameof(newClaim), "Parameter newClaim is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return _usersClaimsTable.ReplaceClaimAsync(user, claim, newClaim);
        }

        public Task RemoveClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion IUserClaimStore<ApplicationUser> 

        #region IUserLockoutStore<ApplicationUser> implementation.
        public Task<DateTimeOffset?> GetLockoutEndDateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.LockoutEndDateTimeUtc.HasValue
                ? new DateTimeOffset?(DateTime.SpecifyKind(user.LockoutEndDateTimeUtc.Value, DateTimeKind.Utc))
                : null);
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.LockoutEndDateTimeUtc = lockoutEnd?.UtcDateTime;
            return Task.FromResult<object>(null);
        }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.AccessFailedCount = 0;
            return Task.FromResult<object>(null);
        }

        public Task<int> GetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.LockoutEnabled = enabled;
            return Task.FromResult<object>(null);
        }
        #endregion IUserLockoutStore<ApplicationUser> implementation.

        #region IUserRoleStore<ApplicationUser> implementation.
        public Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName), "Parameter roleName is not set to an instance of an object.");
            }

            ApplicationRole role = Task.Run(() => _rolesTable.GetAllRoles(), cancellationToken).Result.SingleOrDefault(e => e.NormalizedName == roleName);

            return role != null
                ? _usersRolesTable.AddToRoleAsync(user, role.Id)
                : Task.FromResult<object>(null);
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            ApplicationRole role = Task.Run(() => _rolesTable.GetAllRoles(), cancellationToken).Result.SingleOrDefault(e => e.NormalizedName == roleName);

            return role != null
                ? _usersRolesTable.RemoveFromRoleAsync(user, role.Id)
                : Task.FromResult<object>(null);
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            return _usersRolesTable.GetRolesAsync(user, cancellationToken);
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                return false;
            }

            IList<string> userRoles = await GetRolesAsync(user, cancellationToken);
            return userRoles.Contains(roleName);
        }

        public Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion IUserRoleStore<ApplicationUser> implementation.
    }
}