using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// The default implementation of <see cref="IUserClaimsTable{TKey, TUserClaim}"/>.
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
            const string sql = "SELECT * " +
                               "FROM [dbo].[AspNetUserClaims] " +
                               "WHERE [UserId] = @UserId;";
            var userClaims = await DbConnection.QueryAsync<TUserClaim>(sql, new { UserId = userId });
            return userClaims;
        }
    }
}
