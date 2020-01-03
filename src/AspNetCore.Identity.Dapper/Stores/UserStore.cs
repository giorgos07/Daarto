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
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role and user.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
    /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    /// <typeparam name="TRoleClaim">The type representing a role claim.</typeparam>
    public class UserStore<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> :
        UserStoreBase<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>,
        IProtectedUserStore<TUser>
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
        /// Constructs a new instance of <see cref="UserStore{TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim}"/>.
        /// </summary>
        /// <param name="usersTable">Abstraction for interacting with AspNetUsers table.</param>
        /// <param name="userClaimsTable">Abstraction for interacting with AspNetUserClaims table.</param>
        /// <param name="userRolesTable">Abstraction for interacting with AspNetUserRoles table.</param>
        /// <param name="userLoginsTable">Abstraction for interacting with AspNetUserLogins table.</param>
        /// <param name="userTokensTable">Abstraction for interacting with AspNetUserTokens table.</param>
        /// <param name="rolesTable">Abstraction for interacting with AspNetRoles table.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public UserStore(IUsersTable<TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken> usersTable, IUserClaimsTable<TKey, TUserClaim> userClaimsTable, IUserRolesTable<TRole, TKey, TUserRole> userRolesTable,
            IUserLoginsTable<TUser, TKey, TUserLogin> userLoginsTable, IUserTokensTable<TKey, TUserToken> userTokensTable, IRolesTable<TRole, TKey, TRoleClaim> rolesTable, IdentityErrorDescriber describer) : base(describer) {
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
        public IUsersTable<TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken> UsersTable { get; }
        /// <summary>
        /// Abstraction for interacting with AspNetUserClaims table.
        /// </summary>
        public IUserClaimsTable<TKey, TUserClaim> UserClaimsTable { get; }
        /// <summary>
        /// Abstraction for interacting with AspNetUserRoles table.
        /// </summary>
        public IUserRolesTable<TRole, TKey, TUserRole> UserRolesTable { get; }
        /// <summary>
        /// Abstraction for interacting with AspNetUserLogins table.
        /// </summary>
        public IUserLoginsTable<TUser, TKey, TUserLogin> UserLoginsTable { get; }
        /// <summary>
        /// Abstraction for interacting with AspNetUserTokens table.
        /// </summary>
        public IUserTokensTable<TKey, TUserToken> UserTokensTable { get; }
        /// <summary>
        /// Abstraction for interacting with AspNetRoles table.
        /// </summary>
        public IRolesTable<TRole, TKey, TRoleClaim> RolesTable { get; }

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
        public override async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default) {
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
        public async override Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (string.IsNullOrEmpty(loginProvider)) {
                throw new ArgumentNullException(nameof(loginProvider));
            }
            if (string.IsNullOrEmpty(providerKey)) {
                throw new ArgumentNullException(nameof(providerKey));
            }
            var userLogin = await FindUserLoginAsync(loginProvider, providerKey, cancellationToken);
            if (userLogin != null) {
                return await FindUserAsync(userLogin.UserId, cancellationToken);
            }
            return null;
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
        public override async Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (string.IsNullOrEmpty(normalizedRoleName)) {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }
            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            var users = new List<TUser>();
            if (role != null) {
                users = (await UsersTable.GetUsersInRoleAsync(normalizedRoleName)).ToList();
            }
            return users;
        }

        /// <inheritdoc/>
        public override async Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            if (string.IsNullOrEmpty(normalizedRoleName)) {
                throw new ArgumentException(nameof(normalizedRoleName));
            }
            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (role != null) {
                var userRole = await FindUserRoleAsync(user.Id, role.Id, cancellationToken);
                return userRole != null;
            }
            return false;
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
        public override async Task RemoveFromRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            user.ThrowIfNull(nameof(user));
            if (string.IsNullOrEmpty(normalizedRoleName)) {
                throw new ArgumentException(nameof(normalizedRoleName));
            }
            var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (roleEntity != null) {
                var userRoles = (await UserRolesTable.GetRolesAsync(user.Id))?.Select(x => new TUserRole {
                    UserId = user.Id,
                    RoleId = x.Id
                }).ToList();
                UserRoles = userRoles;
                var userRole = await FindUserRoleAsync(user.Id, roleEntity.Id, cancellationToken);
                if (userRole != null) {
                    UserRoles.Remove(userRole);
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
            var updated = await UsersTable.UpdateAsync(user, UserClaims, UserRoles, UserLogins, UserTokens);
            return updated ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = string.Empty,
                Description = $"User '{user.UserName}' could not be deleted."
            });
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
        protected override async Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var userLogin = await UserLoginsTable.FindUserLoginAsync(loginProvider, providerKey);
            return userLogin;
        }

        /// <inheritdoc/>
        protected override async Task<TUserLogin> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var userLogin = await UserLoginsTable.FindUserLoginAsync(userId, loginProvider, providerKey);
            return userLogin;
        }

        /// <inheritdoc/>
        protected override async Task AddUserTokenAsync(TUserToken token) {
            token.ThrowIfNull(nameof(token));
            UserTokens ??= (await UserTokensTable.GetTokensAsync(token.UserId)).ToList();
            UserTokens.Add(token);
        }

        /// <inheritdoc/>
        protected override async Task RemoveUserTokenAsync(TUserToken token) {
            UserTokens ??= (await UserTokensTable.GetTokensAsync(token.UserId)).ToList();
            UserTokens.Remove(token);
        }

        /// <inheritdoc/>
        protected override Task<TRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var findRoleTask = RolesTable.FindByNameAsync(normalizedRoleName);
            return findRoleTask;
        }

        /// <inheritdoc/>
        protected override async Task<TUserRole> FindUserRoleAsync(TKey userId, TKey roleId, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var userRole = await UserRolesTable.FindUserRoleAsync(userId, roleId);
            return userRole;
        }
    }
}
