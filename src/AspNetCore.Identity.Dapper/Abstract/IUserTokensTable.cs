using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// Abstraction for interacting with AspNetUserTokens table.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    public interface IUserTokensTable<TKey, TUserToken>
        where TKey : IEquatable<TKey>
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        /// <summary>
        /// Gets a list of tokens to be belonging to the specified user.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        Task<IEnumerable<TUserToken>> GetTokensAsync(string userId);
    }
}
