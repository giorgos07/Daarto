using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// The default implementation of <see cref="IUserTokensTable{TKey, TUserToken}"/>.
    /// </summary>
    /// <typeparam name="TDbConnection">The type of the database connection class used to access the store.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    public class UserTokensTable<TDbConnection, TKey, TUserToken> : IUserTokensTable<TKey, TUserToken>
        where TDbConnection : IDbConnection
        where TKey : IEquatable<TKey>
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        /// <summary>
        /// Creates a new instance of <see cref="UserTokensTable{TDbConnection, TKey, TUserToken}"/>.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to use.</param>
        public UserTokensTable(TDbConnection dbConnection) {
            DbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }

        /// <summary>
        /// The <see cref="IDbConnection"/> to use.
        /// </summary>
        protected TDbConnection DbConnection { get; set; }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TUserToken>> GetTokensAsync(TKey userId) {
            const string sql = "SELECT * " +
                               "FROM [dbo].[AspNetUserTokens] " +
                               "WHERE [UserId] = @UserId;";
            var userTokens = await DbConnection.QueryAsync<TUserToken>(sql, new { UserId = userId });
            return userTokens;
        }

        /// <inheritdoc/>
        public virtual async Task<TUserToken> FindTokenAsync(TKey userId, string loginProvider, string name) {
            const string sql = "SELECT * " +
                               "FROM [dbo].[AspNetUserTokens] " +
                               "WHERE [UserId] = @UserId AND [LoginProvider] = @LoginProvider AND [Name] = @Name;";
            var token = await DbConnection.QuerySingleOrDefaultAsync<TUserToken>(sql, new {
                UserId = userId,
                LoginProvider = loginProvider,
                Name = name
            });
            return token;
        }
    }
}
