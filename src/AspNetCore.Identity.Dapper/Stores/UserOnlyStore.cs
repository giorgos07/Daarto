using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// Represents a persistence store for the specified user type, without support for roles.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role and user.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    public class UserOnlyStore<TUser, TKey, TUserClaim, TUserLogin, TUserToken> :
        UserStoreBase<TUser, TKey, TUserClaim, TUserLogin, TUserToken>,
        IProtectedUserStore<TUser>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        /// <summary>
        /// Constructs a new instance of <see cref="UserOnlyStore{TUser, TKey, TUserClaim, TUserLogin, TUserToken}"/>.
        /// </summary>
        /// <param name="usersTable">Abstraction for interacting with AspNetUsers table.</param>
        /// <param name="userClaimsTable">Abstraction for interacting with AspNetUserClaims table.</param>
        /// <param name="userLoginsTable">Abstraction for interacting with AspNetUserLogins table.</param>
        /// <param name="userTokensTable">Abstraction for interacting with AspNetUserTokens table.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public UserOnlyStore(IUsersOnlyTable<TUser, TKey, TUserClaim, TUserLogin, TUserToken> usersTable, IUserClaimsTable<TKey, TUserClaim> userClaimsTable, IUserLoginsTable<TUser, TKey, TUserLogin> userLoginsTable,
            IUserTokensTable<TKey, TUserToken> userTokensTable, IdentityErrorDescriber describer) : base(describer) {
            UsersTable = usersTable ?? throw new ArgumentNullException(nameof(usersTable));
            UserClaimsTable = userClaimsTable ?? throw new ArgumentNullException(nameof(userClaimsTable));
            UserLoginsTable = userLoginsTable ?? throw new ArgumentNullException(nameof(userLoginsTable));
            UserTokensTable = userTokensTable ?? throw new ArgumentNullException(nameof(userTokensTable));
        }

        /// <summary>
        /// Internally keeps the claims of a user.
        /// </summary>
        private IList<TUserClaim> UserClaims { get; set; }
        /// <summary>
        /// Internally keeps the logins of a user.
        /// </summary>
        private IList<TUserLogin> UserLogins { get; set; }
        /// <summary>
        /// Internally keeps the tokens of a user.
        /// </summary>
        private IList<TUserToken> UserTokens { get; set; }
        /// <summary>
        /// Abstraction for interacting with AspNetUsers table.
        /// </summary>
        public IUsersOnlyTable<TUser, TKey, TUserClaim, TUserLogin, TUserToken> UsersTable { get; }
        /// <summary>
        /// Abstraction for interacting with AspNetUserClaims table.
        /// </summary>
        public IUserClaimsTable<TKey, TUserClaim> UserClaimsTable { get; }
        /// <summary>
        /// Abstraction for interacting with AspNetUserLogins table.
        /// </summary>
        public IUserLoginsTable<TUser, TKey, TUserLogin> UserLoginsTable { get; }
        /// <summary>
        /// Abstraction for interacting with AspNetUserTokens table.
        /// </summary>
        public IUserTokensTable<TKey, TUserToken> UserTokensTable { get; }

        /// <inheritdoc/>
        public override IQueryable<TUser> Users => throw new NotSupportedException();

        /// <inheritdoc/>
        public override async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken) {
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            claims.ThrowIfNull(nameof(claims));
            UserClaims ??= (await UserClaimsTable.GetClaimsAsync(user.Id)).ToList();
            foreach (var claim in claims) {
                UserClaims.Add(CreateUserClaim(user, claim));
            }
        }

        /// <inheritdoc/>
        public override async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            login.ThrowIfNull(nameof(login));
            UserLogins ??= (await UserLoginsTable.GetLoginsAsync(user.Id)).ToList();
            UserLogins.Add(CreateUserLogin(user, login));
        }

        /// <inheritdoc/>
        public override async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            var created = await UsersTable.CreateAsync(user);
            return created ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = string.Empty,
                Description = $"User '{user.UserName}' could not be created."
            });
        }

        /// <inheritdoc/>
        public override async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            var deleted = await UsersTable.DeleteAsync(user.Id);
            return deleted ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = string.Empty,
                Description = $"User '{user.UserName}' could not be deleted."
            });
        }

        /// <inheritdoc/>
        public override async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var user = await UsersTable.FindByEmailAsync(normalizedEmail);
            return user;
        }

        /// <inheritdoc/>
        public override async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var id = ConvertIdFromString(userId);
            var user = await UsersTable.FindByIdAsync(id);
            return user;
        }

        /// <inheritdoc/>
        public override async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var user = await UsersTable.FindByNameAsync(normalizedUserName);
            return user;
        }

        /// <inheritdoc/>
        public override async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            var userClaims = await UserClaimsTable.GetClaimsAsync(user.Id);
            return userClaims.Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();
        }

        /// <inheritdoc/>
        public override async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            var userLogins = await UserLoginsTable.GetLoginsAsync(user.Id);
            return userLogins.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName)).ToList();
        }

        /// <inheritdoc/>
        public override async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            claim.ThrowIfNull(nameof(claim));
            var users = await UsersTable.GetUsersForClaimAsync(claim);
            return users.ToList();
        }

        /// <inheritdoc/>
        public override async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken) {
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            claims.ThrowIfNull(nameof(claims));
            UserClaims ??= (await UserClaimsTable.GetClaimsAsync(user.Id)).ToList();
            foreach (var claim in claims) {
                var matchedClaims = UserClaims.Where(x => x.UserId.Equals(user.Id) && x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
                foreach (var matchedClaim in matchedClaims) {
                    UserClaims.Remove(matchedClaim);
                }
            }
        }

        /// <inheritdoc/>
        public override async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            UserLogins ??= (await UserLoginsTable.GetLoginsAsync(user.Id)).ToList();
            var userLogin = await FindUserLoginAsync(user.Id, loginProvider, providerKey, cancellationToken);
            if (userLogin != null) {
                UserLogins.Remove(userLogin);
            }
        }

        /// <inheritdoc/>
        public override async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            claim.ThrowIfNull(nameof(claim));
            newClaim.ThrowIfNull(nameof(newClaim));
            UserClaims ??= (await UserClaimsTable.GetClaimsAsync(user.Id)).ToList();
            var matchedClaims = UserClaims.Where(x => x.UserId.Equals(user.Id) && x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
            foreach (var matchedClaim in matchedClaims) {
                matchedClaim.ClaimValue = newClaim.Value;
                matchedClaim.ClaimType = newClaim.Type;
            }
        }

        /// <inheritdoc/>
        public override async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            var updated = await UsersTable.UpdateAsync(user, UserClaims, UserLogins, UserTokens);
            return updated ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = string.Empty,
                Description = $"User '{user.UserName}' could not be deleted."
            });
        }

        /// <inheritdoc/>
        protected override async Task AddUserTokenAsync(TUserToken token) {
            token.ThrowIfNull(nameof(token));
            UserTokens ??= (await UserTokensTable.GetTokensAsync(token.UserId)).ToList();
            UserTokens.Add(token);
        }

        /// <inheritdoc/>
        protected override async Task<TUserToken> FindTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var token = await UserTokensTable.FindTokenAsync(user.Id, loginProvider, name);
            return token;
        }

        /// <inheritdoc/>
        protected override async Task<TUser> FindUserAsync(TKey userId, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var user = await UsersTable.FindByIdAsync(userId);
            return user;
        }

        /// <inheritdoc/>
        protected override async Task<TUserLogin> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var userLogin = await UserLoginsTable.FindUserLoginAsync(loginProvider, providerKey);
            return userLogin;
        }

        /// <inheritdoc/>
        protected override async Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var userLogin = await UserLoginsTable.FindUserLoginAsync(loginProvider, providerKey);
            return userLogin;
        }

        /// <inheritdoc/>
        protected override async Task RemoveUserTokenAsync(TUserToken token) {
            UserTokens ??= (await UserTokensTable.GetTokensAsync(token.UserId)).ToList();
            UserTokens.Remove(token);
        }
    }
}
