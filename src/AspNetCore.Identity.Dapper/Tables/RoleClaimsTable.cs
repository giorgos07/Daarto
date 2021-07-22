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
    /// The default implementation of <see cref="IRoleClaimsTable{TKey,TRoleClaim}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
    /// <typeparam name="TRoleClaim">The type of the class representing a role claim.</typeparam>
    public class RoleClaimsTable<TKey, TRoleClaim> :
        IdentityTable,
        IRoleClaimsTable<TKey, TRoleClaim>
        where TKey : IEquatable<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        /// <summary>
        /// Creates a new instance of <see cref="RoleClaimsTable{TKey, TRoleClaim}"/>.
        /// </summary>
        /// <param name="dbConnectionFactory">A factory for creating instances of <see cref="IDbConnection"/>.</param>
        public RoleClaimsTable(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TRoleClaim>> GetClaimsAsync(TKey roleId) {
            var query = new Query("AspNetRoleClaims")
                .Where("RoleId", roleId);
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var roleClaims = await dbConnection.QueryAsync<TRoleClaim>(CompileQuery(query));
            return roleClaims;
        }
    }
}
