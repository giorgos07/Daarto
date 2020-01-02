using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// Abstraction for interacting with AspNetUserLogins table.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    public interface IUserLoginsTable<TUser, TKey, TUserLogin>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserLogin : IdentityUserLogin<TKey>, new()
    {
        /// <summary>
        /// Gets a list of external logins to be belonging to the specified user.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        Task<IEnumerable<TUserLogin>> GetLoginsAsync(TKey userId);
        /// <summary>
        /// Finds the user that owns a specified login.
        /// </summary>
        /// <param name="loginProvider">The login provider.</param>
        /// <param name="providerKey">The provider key.</param>
        Task<TUser> FindByLoginAsync(string loginProvider, string providerKey);
        /// <summary>
        /// Finds a user login.
        /// </summary>
        /// <param name="loginProvider">The login provider.</param>
        /// <param name="providerKey">The provider key.</param>
        Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey);
        /// <summary>
        /// Finds a user login.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        /// <param name="loginProvider">The login provider.</param>
        /// <param name="providerKey">The provider key.</param>
        Task<TUserLogin> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey);
    }
}
