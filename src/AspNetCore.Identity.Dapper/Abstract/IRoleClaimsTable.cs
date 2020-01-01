using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// Abstraction for interacting with AspNetRoleClaims table.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
    /// <typeparam name="TRoleClaim">The type of the class representing a role claim.</typeparam>
    public interface IRoleClaimsTable<TKey, TRoleClaim>
        where TKey : IEquatable<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        /// <summary>
        /// Gets a list of claims to be belonging to the specified role.
        /// </summary>
        /// <param name="roleId">The id of the role.</param>
        Task<IEnumerable<TRoleClaim>> GetClaimsAsync(TKey roleId);
    }
}
