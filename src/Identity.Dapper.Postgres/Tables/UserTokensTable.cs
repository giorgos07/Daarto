using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Identity.Dapper.Postgres.Stores;

namespace Identity.Dapper.Postgres.Tables
{
    internal class UserTokensTable
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserTokensTable(IDatabaseConnectionFactory databaseConnectionFactory) => _databaseConnectionFactory = databaseConnectionFactory;

        public async Task<IEnumerable<UserToken>> GetTokensAsync(string userId) {
            const string command = "SELECT * " +
                                   "FROM identity_user_tokens " +
                                   "WHERE user_id = @UserId;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                return await sqlConnection.QueryAsync<UserToken>(command, new {
                    UserId = userId
                });
            }
        }
    }
}
