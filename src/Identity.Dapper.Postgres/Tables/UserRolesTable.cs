using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Identity.Dapper.Postgres.Stores;

namespace Identity.Dapper.Postgres.Tables
{
    internal class UserRolesTable
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserRolesTable(IDatabaseConnectionFactory databaseConnectionFactory) => _databaseConnectionFactory = databaseConnectionFactory;

        public async Task<IEnumerable<UserRole>> GetRolesAsync(ApplicationUser user) {
            const string command = "SELECT r.id AS role_id, r.name AS role_name " +
                                   "FROM identity_roles AS r " +
                                   "INNER JOIN identity_user_roles AS ur ON ur.roleId = r.id " +
                                   "WHERE ur.user_id = @UserId;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                return await sqlConnection.QueryAsync<UserRole>(command, new {
                    UserId = user.Id
                });
            }
        }
    }
}
