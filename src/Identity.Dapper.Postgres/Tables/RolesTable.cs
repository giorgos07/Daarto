using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Identity.Dapper.Postgres.Stores;
using Microsoft.AspNetCore.Identity;

namespace Identity.Dapper.Postgres.Tables
{
    internal class RolesTable
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public RolesTable(IDatabaseConnectionFactory databaseConnectionFactory) => _databaseConnectionFactory = databaseConnectionFactory;

        public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken) {
            const string command = "INSERT INTO identity_roles " +
                                   "(id, name, normalized_name, concurrency_stamp) "+
                                   "VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp);";

            int rowsInserted;

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                rowsInserted = await sqlConnection.ExecuteAsync(command, new {
                    role.Id,
                    role.Name,
                    role.NormalizedName,
                    role.ConcurrencyStamp
                });
            }

            return rowsInserted == 1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = string.Empty,
                Description = $"The role with name {role.Name} could not be inserted."
            });
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationRole role) {
            const string command = "UPDATE identity_roles " +
                                   "SET name = @Name, normalized_name = @NormalizedName, concurrency_stamp = @ConcurrencyStamp " +
                                   "WHERE id = @Id;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                using (var transaction = sqlConnection.BeginTransaction()) {
                    await sqlConnection.ExecuteAsync(command, new {
                        role.Name,
                        role.NormalizedName,
                        role.ConcurrencyStamp,
                        role.Id
                    }, transaction);

                    if (role.Claims.Count() > 0) {
                        const string deleteClaimsCommand = "DELETE " +
                                                           "FROM identity_role_claims " +
                                                           "WHERE role_id = @RoleId;";

                        await sqlConnection.ExecuteAsync(deleteClaimsCommand, new {
                            RoleId = role.Id
                        }, transaction);

                        const string insertClaimsCommand = "INSERT INTO identity_role_claims (role_id, claim_type, claim_value) " +
                                                           "VALUES (@RoleId, @ClaimType, @ClaimValue);";

                        await sqlConnection.ExecuteAsync(insertClaimsCommand, role.Claims.Select(x => new {
                            RoleId = role.Id,
                            ClaimType = x.Type,
                            ClaimValue = x.Value
                        }), transaction);
                    }

                    try {
                        transaction.Commit();
                    } catch {
                        try {
                            transaction.Rollback();
                        } catch {
                            return IdentityResult.Failed(new IdentityError {
                                Code = nameof(UpdateAsync),
                                Description = $"Role with name {role.Name} could not be updated. Operation could not be rolled back."
                            });
                        }

                        return IdentityResult.Failed(new IdentityError {
                            Code = nameof(UpdateAsync),
                            Description = $"Role with name {role.Name} could not be updated.. Operation was rolled back."
                        });
                    }
                }
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationRole role) {
            const string command = "DELETE " +
                                   "FROM identity_roles " +
                                   "WHERE id = @Id;";

            int rowsDeleted;

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                rowsDeleted = await sqlConnection.ExecuteAsync(command, new { role.Id });
            }

            return rowsDeleted == 1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = string.Empty,
                Description = $"The role with name {role.Name} could not be deleted."
            });
        }

        public async Task<ApplicationRole> FindByIdAsync(Guid roleId) {
            const string command = "SELECT * " +
                                   "FROM identity_roles " +
                                   "WHERE id = @Id;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                return await sqlConnection.QuerySingleOrDefaultAsync<ApplicationRole>(command, new {
                    Id = roleId
                });
            }
        }

        public async Task<ApplicationRole> FindByNameAsync(string normalizedRoleName) {
            const string command = "SELECT * " +
                                   "FROM identity_roles " +
                                   "WHERE normalized_name = @NormalizedName;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                return await sqlConnection.QuerySingleOrDefaultAsync<ApplicationRole>(command, new {
                    NormalizedName = normalizedRoleName
                });
            }
        }

        public async Task<IEnumerable<ApplicationRole>> GetAllRolesAsync() {
            const string command = "SELECT * " +
                                   "FROM identity_roles;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                return await sqlConnection.QueryAsync<ApplicationRole>(command);
            }
        }
    }
}
