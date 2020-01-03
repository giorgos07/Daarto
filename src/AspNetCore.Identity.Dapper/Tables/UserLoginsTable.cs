using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// The default implementation of <see cref="IUserLoginsTable{TUser, TKey, TUserLogin}"/>.
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
            const string sql = "SELECT * " +
                               "FROM [dbo].[AspNetUserLogins] " +
                               "WHERE [UserId] = @UserId;";
            var userLogins = await DbConnection.QueryAsync<TUserLogin>(sql, new { UserId = userId });
            return userLogins;
        }

        /// <inheritdoc/>
        public virtual async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey) {
            const string sql = "SELECT [u].* " +
                               "FROM [dbo].[AspNetUsers] AS [u] " +
                               "INNER JOIN [dbo].[AspNetUserLogins] AS [ul] ON [ul].[UserId] = [u].[Id] " +
                               "WHERE [ul].[LoginProvider] = @LoginProvider AND [ul].[ProviderKey] = @ProviderKey;";
            var user = await DbConnection.QuerySingleOrDefaultAsync<TUser>(sql, new {
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            });
            return user;
        }

        /// <inheritdoc/>
        public virtual async Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey) {
            const string sql = "SELECT * " +
                               "FROM [dbo].[AspNetUserLogins] " +
                               "WHERE [LoginProvider] = @LoginProvider AND [ProviderKey] = @ProviderKey;";
            var userLogin = await DbConnection.QuerySingleOrDefaultAsync<TUserLogin>(sql, new {
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            });
            return userLogin;
        }

        /// <inheritdoc/>
        public virtual async Task<TUserLogin> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey) {
            const string sql = "SELECT * " +
                               "FROM [dbo].[AspNetUserLogins] " +
                               "WHERE [UserId] = @UserId AND [LoginProvider] = @LoginProvider AND [ProviderKey] = @ProviderKey;";
            var userLogin = await DbConnection.QuerySingleOrDefaultAsync<TUserLogin>(sql, new {
                UserId = userId,
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            });
            return userLogin;
        }
    }
}
