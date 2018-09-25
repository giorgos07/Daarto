using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace AspNetCore.Identity.Dapper
{
    internal class UserRolesTable
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserRolesTable(IDatabaseConnectionFactory databaseConnectionFactory) => _databaseConnectionFactory = databaseConnectionFactory;

        public async Task AddToRoleAsync(ApplicationUser user, string roleId) {
            const string command = "INSERT INTO dbo.UserRoles " +
                                   "VALUES (@UserId, @RoleId);";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                await sqlConnection.ExecuteAsync(command, new {
                    UserId = user.Id,
                    RoleId = roleId
                });
            }
        }

        public async Task RemoveFromRoleAsync(ApplicationUser user, string roleId) {
            const string command = "DELETE " +
                                   "FROM dbo.UserRoles " +
                                   "WHERE UserId = @UserId AND RoleId = @RoleId;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                await sqlConnection.ExecuteAsync(command, new {
                    UserId = user.Id,
                    RoleId = roleId
                });
            }
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user) {
            const string command = "SELECT r.Name " +
                                   "FROM dbo.Roles AS r " +
                                   "INNER JOIN dbo.UserRoles AS ur ON ur.RoleId = r.Id " +
                                   "WHERE ur.UserId = @UserId;";

            IEnumerable<string> userRoles = new List<string>();

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                userRoles = await sqlConnection.QueryAsync<string>(command, new {
                    UserId = user.Id
                });
            }

            return userRoles.ToList();
        }
    }
}
