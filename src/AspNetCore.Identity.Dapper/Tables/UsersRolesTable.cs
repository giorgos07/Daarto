using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Dapper
{
    internal class UsersRolesTable
    {
        private readonly SqlConnection _sqlConnection;

        public UsersRolesTable(SqlConnection sqlConnection) {
            _sqlConnection = sqlConnection;
        }

        public Task AddToRoleAsync(ApplicationUser user, Guid roleId) {
            const string command = "INSERT INTO dbo.UsersRoles " +
                                   "VALUES (@UserId, @RoleId);";

            return _sqlConnection.ExecuteAsync(command, new {
                UserId = user.Id,
                RoleId = roleId
            });
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, Guid roleId) {
            const string command = "DELETE " +
                                   "FROM dbo.UsersRoles " +
                                   "WHERE UserId = @UserId AND RoleId = @RoleId;";

            return _sqlConnection.ExecuteAsync(command, new {
                UserId = user.Id,
                RoleId = roleId
            });
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken) {
            const string command = "SELECT r.Name " +
                                   "FROM dbo.Roles as r " +
                                   "INNER JOIN dbo.UsersRoles AS ur ON ur.RoleId = r.Id " +
                                   "WHERE ur.UserId = @UserId;";

            var userRoles = Task.Run(() => _sqlConnection.QueryAsync<string>(command, new {
                UserId = user.Id
            }), cancellationToken).Result;

            return Task.FromResult<IList<string>>(userRoles.ToList());
        }
    }
}
