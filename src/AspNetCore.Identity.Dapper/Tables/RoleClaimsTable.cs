using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// The default implementation of <see cref="IRoleClaimsTable{TKey, TRoleClaim}"/>.
    /// </summary>
    /// <typeparam name="TDbConnection">The type of the database connection class used to access the store.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
    /// <typeparam name="TRoleClaim">The type of the class representing a role claim.</typeparam>
    public class RoleClaimsTable<TDbConnection, TKey, TRoleClaim> : IRoleClaimsTable<TKey, TRoleClaim>
        where TDbConnection : IDbConnection
        where TKey : IEquatable<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        private readonly TDbConnection _dbConnection;

        /// <summary>
        /// Creates a new instance of <see cref="RoleClaimsTable{TDbConnection, TKey, TRoleClaim}"/>.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to use.</param>
        public RoleClaimsTable(TDbConnection dbConnection) {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TRoleClaim>> GetClaimsAsync(TKey roleId) {
            const string sql = "SELECT * " +
                               "FROM [dbo].[AspNetRoleClaims] " +
                               "WHERE [RoleId] = @RoleId;";
            var roleClaims = await _dbConnection.QueryAsync<TRoleClaim>(sql, new { RoleId = roleId });
            return roleClaims;
        }
    }
}
