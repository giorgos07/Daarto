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
    /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    public class UserTokensTable<TKey, TUserToken> :
        IdentityTable,
        IUserTokensTable<TKey, TUserToken>
        where TKey : IEquatable<TKey>
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        /// <summary>
        /// Creates a new instance of <see cref="UserTokensTable{TKey, TUserToken}"/>.
        /// </summary>
        /// <param name="dbConnectionFactory">A factory for creating instances of <see cref="IDbConnection"/>.</param>
        public UserTokensTable(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

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
