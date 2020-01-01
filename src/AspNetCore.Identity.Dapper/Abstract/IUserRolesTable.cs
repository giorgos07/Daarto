using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// Abstraction for interacting with AspNetUserRoles table.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    public interface IUserRolesTable<TKey, TRole>
        where TKey : IEquatable<TKey>
        where TRole : IdentityRole<TKey>
    {
        /// <summary>
        /// Gets a list of roles to be belonging to the specified user.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        Task<IEnumerable<TRole>> GetRolesAsync(TKey userId);
    }
}
