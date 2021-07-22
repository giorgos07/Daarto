using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.Dapper;
using AspNetCore.Identity.Dapper.Tables;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace Daarto.WebUI.Data.Tables
{
    public class ExtendedRolesTable : RolesTable<ExtendedIdentityRole, string, IdentityRoleClaim<string>>
    {
        public ExtendedRolesTable(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public override async Task<bool> CreateAsync(ExtendedIdentityRole role) {
            const string sql = "INSERT INTO [dbo].[AspNetRoles] " +
                               "VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp, @Description);";
            using var dbConnection = await DbConnectionFactory.CreateAsync();
            var rowsInserted = await dbConnection.ExecuteAsync(sql, new {
                role.Id,
                role.Name,
                role.NormalizedName,
                role.ConcurrencyStamp,
                role.Description
            });
            return rowsInserted == 1;
        }

        public override async Task<bool> UpdateAsync(ExtendedIdentityRole role, IList<IdentityRoleClaim<string>> claims = null) {
            const string updateRoleSql = "UPDATE [dbo].[AspNetRoles] " +
                                         "SET [Name] = @Name, [NormalizedName] = @NormalizedName, [ConcurrencyStamp] = @ConcurrencyStamp, [Description] = @Description " +
                                         "WHERE [Id] = @Id;";
            using var dbConnection = await DbConnectionFactory.CreateAsync();
            using var transaction = dbConnection.BeginTransaction();
            await dbConnection.ExecuteAsync(updateRoleSql, new {
                role.Name,
                role.NormalizedName,
                role.ConcurrencyStamp,
                role.Description,
                role.Id
            }, transaction);
            if (claims?.Count() > 0) {
                const string deleteClaimsSql = "DELETE " +
                                               "FROM [dbo].[AspNetRoleClaims] " +
                                               "WHERE [RoleId] = @RoleId;";
                await dbConnection.ExecuteAsync(deleteClaimsSql, new {
                    RoleId = role.Id
                }, transaction);
                const string insertClaimsSql = "INSERT INTO [dbo].[AspNetRoleClaims] (RoleId, ClaimType, ClaimValue) " +
                                               "VALUES (@RoleId, @ClaimType, @ClaimValue);";
                await dbConnection.ExecuteAsync(insertClaimsSql, claims.Select(x => new {
                    RoleId = role.Id,
                    x.ClaimType,
                    x.ClaimValue
                }), transaction);
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
