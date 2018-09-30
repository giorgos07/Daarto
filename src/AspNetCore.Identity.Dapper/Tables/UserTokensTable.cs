using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace AspNetCore.Identity.Dapper
{
    internal class UserTokensTable
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserTokensTable(IDatabaseConnectionFactory databaseConnectionFactory) => _databaseConnectionFactory = databaseConnectionFactory;

        public async Task<IEnumerable<UserToken>> GetTokensAsync(string userId) {
            const string command = "SELECT * " +
                                   "FROM dbo.UserTokens " +
                                   "WHERE UserId = @UserId;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                return await sqlConnection.QueryAsync<UserToken>(command, new {
                    UserId = userId
                });
            }
        }
    }
}
