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
    /// The default implementation of <see cref="IUserClaimsTable{TKey,TUserClaim}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
    public class UserClaimsTable<TKey, TUserClaim> :
        IdentityTable,
        IUserClaimsTable<TKey, TUserClaim>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
    {
        /// <summary>
        /// Creates a new instance of <see cref="UserClaimsTable{TKey, TUserClaim}"/>.
        /// </summary>
        /// <param name="dbConnectionFactory">A factory for creating instances of <see cref="IDbConnection"/>.</param>
        public UserClaimsTable(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TUserClaim>> GetClaimsAsync(TKey userId) {
            var query = new Query("AspNetUserClaims")
                .Where("UserId", userId);
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var userClaims = await dbConnection.QueryAsync<TUserClaim>(CompileQuery(query));
            return userClaims;
        }
    }
}
