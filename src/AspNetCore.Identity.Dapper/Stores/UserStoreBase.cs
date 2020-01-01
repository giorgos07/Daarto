using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// Represents a persistence store for the specified user and role types.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key for a role and user.</typeparam>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
    /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    /// <typeparam name="TRoleClaim">The type representing a role claim.</typeparam>
    public abstract class UserStoreBase<TKey, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> :
        UserStoreBase<TKey, TUser, TUserClaim, TUserLogin, TUserToken>,
        IUserRoleStore<TUser>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        /// <summary>
        /// Constructs a new instance of <see cref="UserStoreBase{TKey, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim}"/>.
        /// </summary>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe store errors.</param>
        protected UserStoreBase(IdentityErrorDescriber describer) : base(describer) { }

        /// <inheritdoc/>
        public abstract Task AddToRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract Task RemoveFromRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken);

        /// <summary>
        /// Return a role with the normalized name if it exists.
        /// </summary>
        /// <param name="normalizedRoleName">The normalized role name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The role if it exists.</returns>
        protected abstract Task<TRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken);

        /// <summary>
        /// Called to create a new instance of a <see cref="IdentityUserRole{TKey}"/>.
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="role">The associated role.</param>
        protected virtual TUserRole CreateUserRole(TUser user, TRole role) => new TUserRole {
            UserId = user.Id,
            RoleId = role.Id
        };
    }

    /// <summary>
    /// Represents a persistence store for the specified user and role types.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key for a role and user.</typeparam>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    public abstract class UserStoreBase<TKey, TUser, TUserClaim, TUserLogin, TUserToken> :
        IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserAuthenticationTokenStore<TUser>,
        IUserAuthenticatorKeyStore<TUser>,
        IUserTwoFactorRecoveryCodeStore<TUser>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        private bool _disposed = false;
        private const string InternalLoginProvider = "[AspNetUserStore]";
        private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
        private const string RecoveryCodeTokenName = "RecoveryCodes";

        /// <summary>
        /// Constructs a new instance of <see cref="UserStoreBase{TKey, TUser, TUserClaim, TUserLogin, TUserToken}"/>.
        /// </summary>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe store errors.</param>
        protected UserStoreBase(IdentityErrorDescriber describer) {
            ErrorDescriber = describer ?? new IdentityErrorDescriber();
        }

        /// <summary>
        /// Gets or sets the <see cref="IdentityErrorDescriber"/> for any error that occurred with the current operation.
        /// </summary>
        public IdentityErrorDescriber ErrorDescriber { get; set; }

        /// <inheritdoc/>
        public abstract Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public virtual async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var userLogin = await FindUserLoginAsync(loginProvider, providerKey, cancellationToken);
            if (userLogin != null) {
                return await FindUserAsync(userLogin.UserId, cancellationToken);
            }
            return null;
        }

        /// <inheritdoc/>
        public abstract Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public virtual Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.NormalizedUserName);
        }

        /// <inheritdoc/>
        public virtual Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(ConvertIdToString(user.Id));
        }

        /// <inheritdoc/>
        public virtual Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.UserName);
        }

        /// <inheritdoc/>
        public abstract Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public virtual Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            user.UserName = userName;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public abstract Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.PasswordHash);
        }

        /// <inheritdoc/>
        public virtual Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PasswordHash != null);
        }

        /// <inheritdoc/>
        public virtual Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            stamp.ThrowIfNull(nameof(stamp));
            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.SecurityStamp);
        }

        /// <inheritdoc/>
        public virtual Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            user.Email = email;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.Email);
        }

        /// <inheritdoc/>
        public virtual Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.EmailConfirmed);
        }

        /// <inheritdoc/>
        public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public abstract Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public virtual Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.NormalizedEmail);
        }

        /// <inheritdoc/>
        public virtual Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.LockoutEnd);
        }

        /// <inheritdoc/>
        public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <inheritdoc/>
        public virtual Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <inheritdoc/>
        public virtual Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.LockoutEnabled);
        }

        /// <inheritdoc/>
        public virtual Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.PhoneNumber);
        }

        /// <inheritdoc/>
        public virtual Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        /// <inheritdoc/>
        public virtual Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            return Task.FromResult(user.TwoFactorEnabled);
        }

        /// <inheritdoc/>
        public virtual async Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            var token = await FindTokenAsync(user, loginProvider, name, cancellationToken);
            if (token == null) {
                await AddUserTokenAsync(CreateUserToken(user, loginProvider, name, value));
            } else {
                token.Value = value;
            }
        }

        /// <inheritdoc/>
        public virtual async Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            var entry = await FindTokenAsync(user, loginProvider, name, cancellationToken);
            if (entry != null) {
                await RemoveUserTokenAsync(entry);
            }
        }

        /// <inheritdoc/>
        public virtual async Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            var entry = await FindTokenAsync(user, loginProvider, name, cancellationToken);
            return entry?.Value;
        }

        /// <inheritdoc/>
        public virtual Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
            => SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);

        /// <inheritdoc/>
        public virtual Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
            => GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);

        /// <inheritdoc/>
        public virtual Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken) {
            var mergedCodes = string.Join(";", recoveryCodes);
            return SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            code.ThrowIfNull(nameof(code));
            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? string.Empty;
            var splitCodes = mergedCodes.Split(';');
            if (splitCodes.Contains(code)) {
                var updatedCodes = new List<string>(splitCodes.Where(x => x != code));
                await ReplaceCodesAsync(user, updatedCodes, cancellationToken);
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public virtual async Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? string.Empty;
            if (mergedCodes.Length > 0) {
                return mergedCodes.Split(';').Length;
            }
            return 0;
        }

        /// <summary>
        /// Return a user login with provider, providerKey if it exists.
        /// </summary>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        protected abstract Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken);

        /// <summary>
        /// Return a user with the matching userId if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user if it exists.</returns>
        protected abstract Task<TUser> FindUserAsync(TKey userId, CancellationToken cancellationToken);

        /// <summary>
        /// Find a user token if it exists.
        /// </summary>
        /// <param name="user">The token owner.</param>
        /// <param name="loginProvider">The login provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user token if it exists.</returns>
        protected abstract Task<TUserToken> FindTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken);

        /// <summary>
        /// Add a new user token.
        /// </summary>
        /// <param name="token">The token to be added.</param>
        protected abstract Task AddUserTokenAsync(TUserToken token);

        /// <summary>
        /// Remove a new user token.
        /// </summary>
        /// <param name="token">The token to be removed.</param>
        protected abstract Task RemoveUserTokenAsync(TUserToken token);

        /// <summary>
        /// Called to create a new instance of a <see cref="IdentityUserClaim{TKey}"/>.
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="claim">The associated claim.</param>
        protected virtual TUserClaim CreateUserClaim(TUser user, Claim claim) {
            var userClaim = new TUserClaim {
                UserId = user.Id
            };
            userClaim.InitializeFromClaim(claim);
            return userClaim;
        }

        /// <summary>
        /// Called to create a new instance of a <see cref="IdentityUserLogin{TKey}"/>.
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="login">The sasociated login.</param>
        protected virtual TUserLogin CreateUserLogin(TUser user, UserLoginInfo login) => new TUserLogin {
            UserId = user.Id,
            ProviderKey = login.ProviderKey,
            LoginProvider = login.LoginProvider,
            ProviderDisplayName = login.ProviderDisplayName
        };

        /// <inheritdoc/>
        public void Dispose() {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Converts the provided <paramref name="id"/> to its string representation.
        /// </summary>
        /// <param name="id">The id to convert.</param>
        /// <returns>A <see cref="string"/> representation of the provided <paramref name="id"/>.</returns>
        public virtual string ConvertIdToString(TKey id) {
            if (Equals(id, default(TKey))) {
                return null;
            }
            return id.ToString();
        }

        /// <summary>
        /// Converts the provided <paramref name="id"/> to a strongly typed key object.
        /// </summary>
        /// <param name="id">The id to convert.</param>
        /// <returns>An instance of <typeparamref name="TKey"/> representing the provided <paramref name="id"/>.</returns>
        public virtual TKey ConvertIdFromString(string id) {
            if (id == null) {
                return default;
            }
            return (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(id);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">Indicates whether the method call comes from a Dispose method (its value is true) or from a finalizer (its value is false).</param>
        protected virtual void Dispose(bool disposing) {
            if (_disposed) {
                return;
            }
            if (disposing) {
                // Free any other managed objects here.
            }
            _disposed = true;
        }

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected void ThrowIfDisposed() {
            if (_disposed) {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Called to create a new instance of a <see cref="IdentityUserToken{TKey}"/>.
        /// </summary>
        /// <param name="user">The associated user.</param>
        /// <param name="loginProvider">The associated login provider.</param>
        /// <param name="name">The name of the user token.</param>
        /// <param name="value">The value of the user token.</param>
        protected virtual TUserToken CreateUserToken(TUser user, string loginProvider, string name, string value) => new TUserToken {
            UserId = user.Id,
            LoginProvider = loginProvider,
            Name = name,
            Value = value
        };
    }
}
