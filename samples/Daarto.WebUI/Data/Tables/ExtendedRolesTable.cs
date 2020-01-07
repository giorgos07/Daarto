using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.Dapper;
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
            var rowsInserted = await DbConnection.ExecuteAsync(sql, new {
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
            using (var transaction = DbConnection.BeginTransaction()) {
                await DbConnection.ExecuteAsync(updateRoleSql, new {
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
                    await DbConnection.ExecuteAsync(deleteClaimsSql, new {
                        RoleId = role.Id
                    }, transaction);
                    const string insertClaimsSql = "INSERT INTO [dbo].[AspNetRoleClaims] (RoleId, ClaimType, ClaimValue) " +
                                                   "VALUES (@RoleId, @ClaimType, @ClaimValue);";
                    await DbConnection.ExecuteAsync(insertClaimsSql, claims.Select(x => new {
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
            }
            return true;
        }
    }
}
