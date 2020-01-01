using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// The persistence store for roles.
    /// </summary>
    /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
    public class RoleStore<TRole> : RoleStore<string, TRole>
        where TRole : IdentityRole<string>
    {
        /// <summary>
        /// Constructs a new instance of <see cref="RoleStore{TRole}"/>.
        /// </summary>
        /// <param name="rolesTable">Abstraction for interacting with Roles table.</param>
        /// <param name="roleClaimsTable">Abstraction for interacting with RoleClaims table.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public RoleStore(IRolesTable<string, TRole, IdentityRoleClaim<string>> rolesTable, IRoleClaimsTable<string, IdentityRoleClaim<string>> roleClaimsTable, IdentityErrorDescriber describer = null)
            : base(rolesTable, roleClaimsTable, describer) { }
    }

    /// <summary>
    /// The persistence store for roles.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
    /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
    public class RoleStore<TKey, TRole> :
        RoleStore<TKey, TRole, IdentityUserRole<TKey>, IdentityRoleClaim<TKey>>,
        IRoleClaimStore<TRole>
        where TKey : IEquatable<TKey>
        where TRole : IdentityRole<TKey>
    {
        /// <summary>
        /// Constructs a new instance of <see cref="RoleStore{TKey, TRole}"/>.
        /// </summary>
        /// <param name="rolesTable">Abstraction for interacting with Roles table.</param>
        /// <param name="roleClaimsTable">Abstraction for interacting with RoleClaims table.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public RoleStore(IRolesTable<TKey, TRole, IdentityRoleClaim<TKey>> rolesTable, IRoleClaimsTable<TKey, IdentityRoleClaim<TKey>> roleClaimsTable, IdentityErrorDescriber describer = null)
            : base(rolesTable, roleClaimsTable, describer) { }
    }

    /// <summary>
    /// The persistence store for roles.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
    /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
    /// <typeparam name="TUserRole">The type of the class representing a user role.</typeparam>
    /// <typeparam name="TRoleClaim">The type of the class representing a role claim.</typeparam>
    public class RoleStore<TKey, TRole, TUserRole, TRoleClaim> : IRoleClaimStore<TRole>
        where TKey : IEquatable<TKey>
        where TRole : IdentityRole<TKey>
        where TUserRole : IdentityUserRole<TKey>, new()
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        private bool _disposed = false;

        /// <summary>
        /// Constructs a new instance of <see cref="RoleStore{TKey, TRole, TUserRole, TRoleClaim}"/>.
        /// </summary>
        /// <param name="rolesTable">Abstraction for interacting with Roles table.</param>
        /// <param name="roleClaimsTable">Abstraction for interacting with RoleClaims table.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public RoleStore(IRolesTable<TKey, TRole, TRoleClaim> rolesTable, IRoleClaimsTable<TKey, TRoleClaim> roleClaimsTable, IdentityErrorDescriber describer = null) {
            RolesTable = rolesTable ?? throw new ArgumentNullException(nameof(rolesTable));
            RoleClaimsTable = roleClaimsTable ?? throw new ArgumentNullException(nameof(roleClaimsTable));
            ErrorDescriber = describer ?? new IdentityErrorDescriber();
        }

        /// <summary>
        /// Abstraction for interacting with Roles table.
        /// </summary>
        private IRolesTable<TKey, TRole, TRoleClaim> RolesTable { get; set; }
        /// <summary>
        /// Abstraction for interacting with RoleClaims table.
        /// </summary>
        private IRoleClaimsTable<TKey, TRoleClaim> RoleClaimsTable { get; set; }
        /// <summary>
        /// Internally keeps the claims of a role.
        /// </summary>
        private IList<TRoleClaim> RoleClaims { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="IdentityErrorDescriber"/> for any error that occurred with the current operation.
        /// </summary>
        public IdentityErrorDescriber ErrorDescriber { get; set; }

        /// <inheritdoc/>
        public virtual async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            claim.ThrowIfNull(nameof(claim));
            RoleClaims ??= (await RoleClaimsTable.GetClaimsAsync(role.Id)).ToList();
            RoleClaims.Add(CreateRoleClaim(role, claim));
        }

        /// <inheritdoc/>
        public virtual async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            var created = await RolesTable.CreateAsync(role);
            return created ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = string.Empty,
                Description = $"Role '{role.Name}' could not be created."
            });
        }

        /// <inheritdoc/>
        public virtual async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            var deleted = await RolesTable.DeleteAsync(role.Id);
            return deleted ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = string.Empty,
                Description = $"Role '{role.Name}' could not be deleted."
            });
        }

        /// <inheritdoc/>
        public virtual async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var id = ConvertIdFromString(roleId);
            var role = await RolesTable.FindByIdAsync(id);
            return role;
        }

        /// <inheritdoc/>
        public virtual async Task<TRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var role = await RolesTable.FindByNameAsync(normalizedName);
            return role;
        }

        /// <inheritdoc/>
        public virtual async Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            var roleClaims = await RoleClaimsTable.GetClaimsAsync(role.Id);
            return roleClaims.Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();
        }

        /// <inheritdoc/>
        public virtual Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            return Task.FromResult(role.NormalizedName);
        }

        /// <inheritdoc/>
        public virtual Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            return Task.FromResult(ConvertIdToString(role.Id));
        }

        /// <inheritdoc/>
        public virtual Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            return Task.FromResult(role.Name);
        }

        /// <inheritdoc/>
        public virtual async Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            claim.ThrowIfNull(nameof(role));
            RoleClaims ??= (await RoleClaimsTable.GetClaimsAsync(role.Id)).ToList();
            var roleClaims = RoleClaims.Where(x => x.RoleId.Equals(role.Id) && x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
            foreach (var roleClaim in RoleClaims) {
                RoleClaims.Remove(roleClaim);
            }
        }

        /// <inheritdoc/>
        public virtual Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            role.Name = roleName;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            role.ConcurrencyStamp = Guid.NewGuid().ToString();
            var updated = await RolesTable.UpdateAsync(role, RoleClaims);
            return updated ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = string.Empty,
                Description = $"Role '{role.Name}' could not be deleted."
            });
        }

        /// <inheritdoc/>
        public void Dispose() {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
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
        /// Converts the provided <paramref name="id"/> to its string representation.
        /// </summary>
        /// <param name="id">The id to convert.</param>
        /// <returns>An <see cref="string"/> representation of the provided <paramref name="id"/>.</returns>
        public virtual string ConvertIdToString(TKey id) {
            if (id.Equals(default)) {
                return null;
            }
            return id.ToString();
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
        /// Creates an entity representing a role claim.
        /// </summary>
        /// <param name="role">The associated role.</param>
        /// <param name="claim">The associated claim.</param>
        /// <returns>The role claim entity.</returns>
        protected virtual TRoleClaim CreateRoleClaim(TRole role, Claim claim) => new TRoleClaim {
            RoleId = role.Id,
            ClaimType = claim.Type,
            ClaimValue = claim.Value
        };
    }
}
