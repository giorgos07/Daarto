using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// The default implementation of <see cref="IUserLoginsTable{TKey, TUser, TUserLogin}"/>.
    /// </summary>
    /// <typeparam name="TDbConnection">The type of the database connection class used to access the store.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    public class UserLoginsTable<TDbConnection, TKey, TUser, TUserLogin> : IUserLoginsTable<TKey, TUser, TUserLogin>
        where TDbConnection : IDbConnection
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TKey>
        where TUserLogin : IdentityUserLogin<TKey>, new()
    {
        /// <summary>
        /// Creates a new instance of <see cref="UserLoginsTable{TDbConnection, TKey, TUser, TUserLogin}"/>.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to use.</param>
        public UserLoginsTable(TDbConnection dbConnection) {
            DbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }

        /// <summary>
        /// The <see cref="IDbConnection"/> to use.
        /// </summary>
        protected TDbConnection DbConnection { get; set; }

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
            string[] sql =
            {
                "SELECT [UserId] " +
                "FROM [dbo].[AspNetUserLogins] " +
                "WHERE [LoginProvider] = @LoginProvider AND [ProviderKey] = @ProviderKey;"
            };
            var userId = await DbConnection.QuerySingleOrDefaultAsync<TKey>(sql[0], new {
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            });
            if (userId == null) {
                return null;
            }
            sql[0] = "SELECT * " +
                     "FROM [dbo].[AspNetUsers] " +
                     "WHERE [Id] = @Id;";
            var user = await DbConnection.QuerySingleAsync<TUser>(sql[0], new { Id = userId });
            return user;
        }
    }
}
