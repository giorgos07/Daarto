using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    internal class RolesTable
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public RolesTable(IDatabaseConnectionFactory databaseConnectionFactory) => _databaseConnectionFactory = databaseConnectionFactory;

        public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken) {
            const string command = "INSERT INTO dbo.Roles " +
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
            const string command = "UPDATE dbo.Roles " +
                                   "SET Name = @Name, NormalizedName = @NormalizedName, ConcurrencyStamp = @ConcurrencyStamp " +
                                   "WHERE Id = @Id;";

            int rowsUpdated;

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                rowsUpdated = await sqlConnection.ExecuteAsync(command, new {
                    role.Name,
                    role.NormalizedName,
                    role.ConcurrencyStamp,
                    role.Id
                });
            }

            return rowsUpdated == 1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = string.Empty,
                Description = $"The role with name {role.Name} could not be updated."
            });
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationRole role) {
            const string command = "DELETE " +
                                   "FROM dbo.Roles " +
                                   "WHERE Id = @Id;";

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
                                   "FROM dbo.Roles " +
                                   "WHERE Id = @Id;";

            ApplicationRole role;

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                role = await sqlConnection.QuerySingleOrDefaultAsync<ApplicationRole>(command, new {
                    Id = roleId
                });
            }

            return role;
        }

        public async Task<ApplicationRole> FindByNameAsync(string normalizedRoleName) {
            const string command = "SELECT * " +
                                   "FROM dbo.Roles " +
                                   "WHERE NormalizedName = @NormalizedName;";

            ApplicationRole role;

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                role = await sqlConnection.QuerySingleOrDefaultAsync<ApplicationRole>(command, new {
                    NormalizedName = normalizedRoleName
                });
            }

            return role;
        }

        public async Task<IEnumerable<ApplicationRole>> GetAllRoles() {
            const string command = "SELECT * " +
                                   "FROM dbo.Roles;";

            IEnumerable<ApplicationRole> roles = new List<ApplicationRole>();

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                roles = await sqlConnection.QueryAsync<ApplicationRole>(command);
            }

            return roles;
        }
    }
}
