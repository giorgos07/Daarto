using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// Abstraction for interacting with AspNetRoles table.
    /// </summary>
    /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
    /// <typeparam name="TRoleClaim">The type of the class representing a role claim.</typeparam>
    public interface IRolesTable<TRole, TKey, TRoleClaim>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        /// <summary>
        /// Creates a new role in the store.
        /// </summary>
        /// <param name="role">The role to create in the store.</param>
        Task<bool> CreateAsync(TRole role);
        /// <summary>
        /// Deletes a role from the store.
        /// </summary>
        /// <param name="roleId">The id of the role to delete from the store.</param>
        Task<bool> DeleteAsync(TKey roleId);
        /// <summary>
        /// Finds the role who has the specified id.
        /// </summary>
        /// <param name="roleId">The role id to look for.</param>
        Task<TRole> FindByIdAsync(TKey roleId);
        /// <summary>
        /// Finds the role who has the specified normalized name.
        /// </summary>
        /// <param name="normalizedName">The normalized role name to look for.</param>
        Task<TRole> FindByNameAsync(string normalizedName);
        /// <summary>
        /// Updates a role in the store.
        /// </summary>
        /// <param name="role">The role to update in the store.</param>
        /// <param name="claims">The claims of the role.</param>
        Task<bool> UpdateAsync(TRole role, IList<TRoleClaim> claims = null);
    }
}
