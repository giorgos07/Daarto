using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace AspNetCore.Identity.Dapper
{
    internal class UserRolesTable
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserRolesTable(IDatabaseConnectionFactory databaseConnectionFactory) => _databaseConnectionFactory = databaseConnectionFactory;

        public async Task<IEnumerable<UserRole>> GetRolesAsync(ApplicationUser user) {
            const string command = "SELECT r.Id AS RoleId, r.Name AS RoleName " +
                                   "FROM dbo.Roles AS r " +
                                   "INNER JOIN dbo.UserRoles AS ur ON ur.RoleId = r.Id " +
                                   "WHERE ur.UserId = @UserId;";

            IEnumerable<UserRole> userRoles = new List<UserRole>();

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                userRoles = await sqlConnection.QueryAsync<UserRole>(command, new {
                    UserId = user.Id
                });
            }

            return userRoles;
        }
    }
}
