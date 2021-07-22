using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;
using SqlKata;

namespace AspNetCore.Identity.Dapper.Tables
{
    /// <summary>
    /// The default implementation of <see cref="IUserLoginsTable{TUser,TKey,TUserLogin}"/>.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    public class UserLoginsTable<TUser, TKey, TUserLogin> :
        IdentityTable,
        IUserLoginsTable<TUser, TKey, TUserLogin>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserLogin : IdentityUserLogin<TKey>, new()
    {
        /// <summary>
        /// Creates a new instance of <see cref="UserLoginsTable{TUser, TKey, TUserLogin}"/>.
        /// </summary>
        /// <param name="dbConnectionFactory">A factory for creating instances of <see cref="IDbConnection"/>.</param>
        public UserLoginsTable(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TUserLogin>> GetLoginsAsync(TKey userId) {
            var query = new Query("AspNetUserLogins")
                .Where("UserId", userId);
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var userLogins = await dbConnection.QueryAsync<TUserLogin>(CompileQuery(query));
            return userLogins;
        }

        /// <inheritdoc/>
        public virtual async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey)
        {
            var query = new Query("AspNetUsers as u")
                .Join("AspNetUserLogins ul", "u.Id", "ul.Id")
                .Where("ul.LoginProvider", loginProvider)
                .Where("ul.ProviderKey", providerKey);
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var user = await dbConnection.QuerySingleOrDefaultAsync<TUser>(CompileQuery(query));
            return user;
        }

        /// <inheritdoc/>
        public virtual async Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey) {
            var query = new Query("AspNetUserLogins")
                .Where("LoginProvider", loginProvider)
                .Where("ProviderKey", providerKey);
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var userLogin = await dbConnection.QuerySingleOrDefaultAsync<TUserLogin>(CompileQuery(query));
            return userLogin;
        }

        /// <inheritdoc/>
        public virtual async Task<TUserLogin> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey) {
            var query = new Query("AspNetUserLogins")
                .Where("LoginProvider", loginProvider)
                .Where("ProviderKey", providerKey)
                .Where("UserId", userId);
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var userLogin = await dbConnection.QuerySingleOrDefaultAsync<TUserLogin>(CompileQuery(query));
            return userLogin;
        }
    }
}
