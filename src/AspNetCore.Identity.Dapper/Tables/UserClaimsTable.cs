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
    /// <typeparam name="TDbConnection">The type of the database connection class used to access the store.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
    public class UserClaimsTable<TDbConnection, TKey, TUserClaim> : IUserClaimsTable<TKey, TUserClaim>
        where TDbConnection : IDbConnection
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
    {
        /// <summary>
        /// Creates a new instance of <see cref="UserClaimsTable{TDbConnection, TKey, TUserClaim}"/>.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to use.</param>
        public UserClaimsTable(TDbConnection dbConnection) {
            DbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }

        /// <summary>
        /// The <see cref="IDbConnection"/> to use.
        /// </summary>
        protected TDbConnection DbConnection { get; set; }

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
