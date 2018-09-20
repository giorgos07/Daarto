using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    internal class UsersLoginsTable
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UsersLoginsTable(IDatabaseConnectionFactory databaseConnectionFactory) => _databaseConnectionFactory = databaseConnectionFactory;

        public async Task AddLoginAsync(ApplicationUser user, UserLoginInfo login) {
            const string command = "INSERT INTO dbo.UsersLogins " +
                                   "VALUES (@LoginProvider, @ProviderKey, @UserId, @ProviderDisplayName);";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                await sqlConnection.ExecuteAsync(command, new {
                    login.LoginProvider,
                    login.ProviderKey,
                    UserId = user.Id,
                    login.ProviderDisplayName
                });
            }
        }

        public async Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey) {
            const string command = "DELETE " +
                                   "FROM dbo.UsersLogins " +
                                   "WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                await sqlConnection.ExecuteAsync(command, new {
                    UserId = user.Id,
                    LoginProvider = loginProvider,
                    ProviderKey = providerKey
                });
            }
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user) {
            const string command = "SELECT * " +
                                   "FROM dbo.UsersLogins " +
                                   "WHERE UserId = @UserId;";

            IEnumerable<UserLogin> userLogins = new List<UserLogin>();

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                userLogins = await sqlConnection.QueryAsync<UserLogin>(command, new {
                    UserId = user.Id
                });
            }

            return userLogins.Select(e => new UserLoginInfo(e.LoginProvider, e.ProviderKey, e.ProviderDisplayName)).ToList();
        }

        public async Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey) {
            string[] command =
            {
                "SELECT UserId " +
                "FROM dbo.UsersLogins " +
                "WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey;"
            };

            ApplicationUser user;

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                var userId = await sqlConnection.QuerySingleOrDefaultAsync<Guid?>(command[0], new {
                    LoginProvider = loginProvider,
                    ProviderKey = providerKey
                });

                if (userId == null) {
                    return null;
                }

                command[0] = "SELECT * " +
                             "FROM dbo.Users " +
                             "WHERE Id = @Id;";

                user = await sqlConnection.QuerySingleAsync<ApplicationUser>(command[0], new { Id = userId });
            }

            return user;
        }
    }
}
