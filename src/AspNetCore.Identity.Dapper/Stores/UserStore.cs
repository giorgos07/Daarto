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
    public class UserStore<TKey, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> :
        UserStoreBase<TKey, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>,
        IProtectedUserStore<TUser>
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        /// <summary>
        /// Constructs a new instance of <see cref="UserStore{TKey, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim}"/>.
        /// </summary>
        /// <param name="usersTable">Abstraction for interacting with AspNetUsers table.</param>
        /// <param name="userClaimsTable">Abstraction for interacting with AspNetUserClaims table.</param>
        /// <param name="userRolesTable">Abstraction for interacting with AspNetUserRoles table.</param>
        /// <param name="userLoginsTable">Abstraction for interacting with AspNetUserLogins table.</param>
        /// <param name="userTokensTable">Abstraction for interacting with AspNetUserTokens table.</param>
        /// <param name="rolesTable">Abstraction for interacting with AspNetRoles table.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public UserStore(IUsersTable<TKey, TUser, TUserClaim, TUserRole, TUserLogin, TUserToken> usersTable, IUserClaimsTable<TKey, TUserClaim> userClaimsTable, IUserRolesTable<TKey, TRole> userRolesTable,
            IUserLoginsTable<TKey, TUser, TUserLogin> userLoginsTable, IUserTokensTable<TKey, TUserToken> userTokensTable, IRolesTable<TKey, TRole, TRoleClaim> rolesTable, IdentityErrorDescriber describer = null) : base(describer) {
            UsersTable = usersTable ?? throw new ArgumentNullException(nameof(usersTable));
            UserClaimsTable = userClaimsTable ?? throw new ArgumentNullException(nameof(userClaimsTable));
            UserRolesTable = userRolesTable ?? throw new ArgumentNullException(nameof(userRolesTable));
            UserLoginsTable = userLoginsTable ?? throw new ArgumentNullException(nameof(userLoginsTable));
            UserTokensTable = userTokensTable ?? throw new ArgumentNullException(nameof(userTokensTable));
            RolesTable = rolesTable ?? throw new ArgumentNullException(nameof(rolesTable));
        }

        /// <summary>
        /// Internally keeps the claims of a user.
        /// </summary>
        private IList<TUserClaim> UserClaims { get; set; }
        /// <summary>
        /// Internally keeps the roles of a user.
        /// </summary>
        private IList<TUserRole> UserRoles { get; set; }
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
        public IUsersTable<TKey, TUser, TUserClaim, TUserRole, TUserLogin, TUserToken> UsersTable { get; }
        /// <summary>
        /// Abstraction for interacting with AspNetUserClaims table.
        /// </summary>
        public IUserClaimsTable<TKey, TUserClaim> UserClaimsTable { get; }
        /// <summary>
        /// Abstraction for interacting with AspNetUserRoles table.
        /// </summary>
        public IUserRolesTable<TKey, TRole> UserRolesTable { get; }
        /// <summary>
        /// Abstraction for interacting with AspNetUserLogins table.
        /// </summary>
        public IUserLoginsTable<TKey, TUser, TUserLogin> UserLoginsTable { get; }
        /// <summary>
        /// Abstraction for interacting with AspNetUserTokens table.
        /// </summary>
        public IUserTokensTable<TKey, TUserToken> UserTokensTable { get; }
        /// <summary>
        /// Abstraction for interacting with AspNetRoles table.
        /// </summary>
        public IRolesTable<TKey, TRole, TRoleClaim> RolesTable { get; }

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
        public override async Task AddToRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            if (string.IsNullOrEmpty(normalizedRoleName)) {
                throw new ArgumentException($"Parameter {nameof(normalizedRoleName)} cannot be null or empty.");
            }
            var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (roleEntity == null) {
                throw new InvalidOperationException($"Role '{normalizedRoleName}' was not found.");
            }
            var userRoles = (await UserRolesTable.GetRolesAsync(user.Id))?.Select(x => new TUserRole {
                UserId = user.Id,
                RoleId = x.Id
            }).ToList();
            UserRoles = userRoles;
            UserRoles.Add(CreateUserRole(user, roleEntity));
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
        public override async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            var userRoles = await UserRolesTable.GetRolesAsync(user.Id);
            return userRoles.Select(x => x.Name).ToList();
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
        public override Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task RemoveFromRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override Task AddUserTokenAsync(TUserToken token) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override Task<TUserToken> FindTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override Task<TUser> FindUserAsync(TKey userId, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override Task RemoveUserTokenAsync(TUserToken token) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override Task<TRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken) =>
            RolesTable.FindByNameAsync(normalizedRoleName);
    }
}
