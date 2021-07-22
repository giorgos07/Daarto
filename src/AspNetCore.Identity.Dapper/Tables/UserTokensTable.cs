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
    /// The default implementation of <see cref="IUserTokensTable{TKey,TUserToken}"/>.
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
            var query = new Query("AspNetUserTokens")
                .Where("UserId", userId);
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var userTokens = await dbConnection.QueryAsync<TUserToken>(CompileQuery(query));
            return userTokens;
        }

        /// <inheritdoc/>
        public virtual async Task<TUserToken> FindTokenAsync(TKey userId, string loginProvider, string name) {
            var query = new Query("AspNetUserTokens")
                .Where("UserId", userId)
                .Where("LoginProvider", loginProvider)
                .Where("Name", name);
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var token = await dbConnection.QuerySingleOrDefaultAsync<TUserToken>(CompileQuery(query));
            return token;
        }
    }
}
