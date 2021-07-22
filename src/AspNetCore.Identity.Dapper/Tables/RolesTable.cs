using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;
using SqlKata;

namespace AspNetCore.Identity.Dapper.Tables
{
    /// <summary>
    /// The default implementation of <see cref="IRolesTable{TRole,TKey,TRoleClaim}"/>.
    /// </summary>
    /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
    /// <typeparam name="TRoleClaim">The type of the class representing a role claim.</typeparam>
    public class RolesTable<TRole, TKey, TRoleClaim> :
        IdentityTable,
        IRolesTable<TRole, TKey, TRoleClaim>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        /// <summary>
        /// Creates a new instance of <see cref="RolesTable{TRole, TKey, TRoleClaim}"/>.
        /// </summary>
        /// <param name="dbConnectionFactory">A factory for creating instances of <see cref="IDbConnection"/>.</param>
        public RolesTable(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        /// <inheritdoc/>
        public virtual async Task<bool> CreateAsync(TRole role)
        {
            var query = new Query("AspNetRoles")
                .AsInsert(new
                {
                    role.Id,
                    role.Name,
                    role.NormalizedName,
                    role.ConcurrencyStamp
                });
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var rowsInserted = await dbConnection.ExecuteAsync(CompileQuery(query));
            return rowsInserted == 1;
        }

        /// <inheritdoc/>
        public virtual async Task<bool> DeleteAsync(TKey roleId)
        {
            var query = new Query("AspNetRoles")
                .Where("Id", roleId)
                .AsDelete();
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var rowsDeleted = await dbConnection.ExecuteAsync(CompileQuery(query));
            return rowsDeleted == 1;
        }

        /// <inheritdoc/>
        public virtual async Task<TRole> FindByIdAsync(TKey roleId)
        {
            var query = new Query("AspNetRoles")
                .Where("Id", roleId);
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var role = await dbConnection.QuerySingleOrDefaultAsync<TRole>(CompileQuery(query));
            return role;
        }

        /// <inheritdoc/>
        public virtual async Task<TRole> FindByNameAsync(string normalizedName) {
            var query = new Query("AspNetRoles")
                .Where("NormalizedName", normalizedName);
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var role = await dbConnection.QuerySingleOrDefaultAsync<TRole>(CompileQuery(query));
            return role;
        }

        /// <inheritdoc/>
        public virtual async Task<bool> UpdateAsync(TRole role, IList<TRoleClaim> claims = null) {
            var updateRoleSqlQuery = new Query("AspNetRoles")
                .Where("Id", role.Id)
                .AsUpdate(new
                {
                    role.Name,
                    role.NormalizedName,
                    role.ConcurrencyStamp,
                    role.Id
                });
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            using var transaction = dbConnection.BeginTransaction();
            await dbConnection.ExecuteAsync(CompileQuery(updateRoleSqlQuery), transaction);
            if (claims?.Count() > 0) {
                var deleteClaimsSqlQuery = new Query("AspNetRoleClaims")
                    .Where("RoleId", role.Id)
                    .AsDelete();
                await dbConnection.ExecuteAsync(CompileQuery(deleteClaimsSqlQuery), transaction);
                var insertClaimsSqlQuery =new Query("AspNetRoleClaims")
                    .AsInsert(
                        new[]
                        {
                            "RoleId",
                            "ClaimType",
                            "ClaimValue"
                        },
                        claims.Select(x => new object[]
                        {
                            role.Id,
                            x.ClaimType,
                            x.ClaimValue
                        }).ToArray());
                    
                await dbConnection.ExecuteAsync(CompileQuery(insertClaimsSqlQuery), transaction);
            }
            try {
                transaction.Commit();
            } catch {
                transaction.Rollback();
                return false;
            }

            return true;
        }
    }
}
