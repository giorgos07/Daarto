using Daarto.IdentityProvider.Entities;
using Dapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Daarto.IdentityProvider.Tables
{
    public class RolesTable
    {
        private SqlConnection _sqlConnection;

        public RolesTable(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        public Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            const string command = "INSERT INTO dbo.Roles " +
                                   "VALUES (@Id, @ConcurrencyStamp, @Name, @NormalizedName);";

            int rowsInserted = Task.Run(() => _sqlConnection.ExecuteAsync(command, new
            {
                role.Id,
                role.ConcurrencyStamp,
                role.Name,
                role.NormalizedName
            }), cancellationToken).Result;

            return Task.FromResult(rowsInserted.Equals(1) ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
            {
                Code = string.Empty,
                Description = $"The role with name {role.Name} could not be inserted in the dbo.Roles table."
            }));
        }

        public Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            const string command = "UPDATE dbo.Roles " +
                                   "SET ConcurrencyStamp = @ConcurrencyStamp, Name = @Name, NormalizedName = @NormalizedName " +
                                   "WHERE Id = @Id;";

            int rowsUpdated = Task.Run(() => _sqlConnection.ExecuteAsync(command, new
            {
                role.ConcurrencyStamp,
                role.Name,
                role.NormalizedName,
                role.Id
            }), cancellationToken).Result;

            return Task.FromResult(rowsUpdated.Equals(1) ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
            {
                Code = string.Empty,
                Description = $"The role with name {role.Name} could not be updated in the dbo.Roles table."
            }));
        }

        public Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            const string command = "DELETE " +
                                   "FROM dbo.Roles " +
                                   "WHERE Id = @Id;";

            int rowsDeleted = Task.Run(() => _sqlConnection.ExecuteAsync(command, new { role.Id }), cancellationToken).Result;

            return Task.FromResult(rowsDeleted.Equals(1) ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
            {
                Code = string.Empty,
                Description = $"The role with name {role.Name} could not be deleted from the dbo.Roles table."
            }));
        }

        public Task<ApplicationRole> FindByIdAsync(Guid roleId)
        {
            const string command = "SELECT * " +
                                   "FROM dbo.Roles " +
                                   "WHERE Id = @Id;";

            return _sqlConnection.QuerySingleOrDefaultAsync<ApplicationRole>(command, new
            {
                Id = roleId
            });
        }

        public Task<ApplicationRole> FindByNameAsync(string normalizedRoleName)
        {
            const string command = "SELECT * " +
                                   "FROM dbo.Roles " +
                                   "WHERE NormalizedName = @NormalizedName;";

            return _sqlConnection.QuerySingleOrDefaultAsync<ApplicationRole>(command, new
            {
                NormalizedName = normalizedRoleName
            });
        }

        public Task<IEnumerable<ApplicationRole>> GetAllRoles()
        {
            const string command = "SELECT * " +
                                   "FROM dbo.Roles;";

            return _sqlConnection.QueryAsync<ApplicationRole>(command);
        }

        public void Dispose()
        {
            if (_sqlConnection == null)
            {
                return;
            }

            _sqlConnection.Dispose();
            _sqlConnection = null;
        }
    }
}