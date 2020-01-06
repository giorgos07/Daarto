using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// Abstraction for interacting with AspNetUsers table.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role and user.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    public interface IUsersOnlyTable<TUser, TKey, TUserClaim, TUserLogin, TUserToken>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        /// <summary>
        /// Creates a new user in the store.
        /// </summary>
        /// <param name="user">The user to create in the store.</param>
        Task<bool> CreateAsync(TUser user);
        /// <summary>
        /// Deletes a user from the store.
        /// </summary>
        /// <param name="userId">The id of the user to delete from the store.</param>
        Task<bool> DeleteAsync(TKey userId);
        /// <summary>
        /// Finds the user who has the specified id.
        /// </summary>
        /// <param name="userId">The user id to look for.</param>
        Task<TUser> FindByIdAsync(TKey userId);
        /// <summary>
        /// Finds the user who has the specified username.
        /// </summary>
        /// <param name="normalizedUserName">The username to look for.</param>
        Task<TUser> FindByNameAsync(string normalizedUserName);
        /// <summary>
        /// Finds the user who has the specified email.
        /// </summary>
        /// <param name="normalizedEmail">The email to look for.</param>
        Task<TUser> FindByEmailAsync(string normalizedEmail);
        /// <summary>
        /// Updates a user in the store.
        /// </summary>
        /// <param name="user">The user to update in the store.</param>
        /// <param name="claims">The claims of the user.</param>
        /// <param name="logins">The logins of the user.</param>
        /// <param name="tokens">The tokens of the user.</param>
        Task<bool> UpdateAsync(TUser user, IList<TUserClaim> claims, IList<TUserLogin> logins, IList<TUserToken> tokens);
        /// <summary>
        /// Gets the users that own the specified claim.
        /// </summary>
        /// <param name="claim">The claim to look for.</param>
        Task<IEnumerable<TUser>> GetUsersForClaimAsync(Claim claim);
    }

    /// <summary>
    /// Abstraction for interacting with AspNetUsers table.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role and user.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
    /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    public interface IUsersTable<TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken> : IUsersOnlyTable<TUser, TKey, TUserClaim, TUserLogin, TUserToken>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        /// <summary>
        /// Updates a user in the store.
        /// </summary>
        /// <param name="user">The user to update in the store.</param>
        /// <param name="claims">The claims of the user.</param>
        /// <param name="roles">The roles of the user.</param>
        /// <param name="logins">The logins of the user.</param>
        /// <param name="tokens">The tokens of the user.</param>
        Task<bool> UpdateAsync(TUser user, IList<TUserClaim> claims, IList<TUserRole> roles, IList<TUserLogin> logins, IList<TUserToken> tokens);
        /// <summary>
        /// Gets the users that belong to the specified role.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        Task<IEnumerable<TUser>> GetUsersInRoleAsync(string roleName);
    }
}
