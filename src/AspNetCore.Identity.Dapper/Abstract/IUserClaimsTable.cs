using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// Abstraction for interacting with AspNetUserClaims table.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
    public interface IUserClaimsTable<TKey, TUserClaim>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
    {
        /// <summary>
        /// Gets a list of claims to be belonging to the specified user.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        Task<IEnumerable<TUserClaim>> GetClaimsAsync(TKey userId);
    }
}
