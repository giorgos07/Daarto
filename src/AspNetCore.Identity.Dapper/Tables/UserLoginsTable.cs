using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    internal class UserLoginsTable
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserLoginsTable(IDatabaseConnectionFactory databaseConnectionFactory) => _databaseConnectionFactory = databaseConnectionFactory;

        public async Task AddLoginAsync(ApplicationUser user, UserLoginInfo login) {
            const string command = "INSERT INTO dbo.UserLogins " +
                                   "VALUES (@LoginProvider, @ProviderKey, @ProviderDisplayName, @UserId);";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                await sqlConnection.ExecuteAsync(command, new {
                    login.LoginProvider,
                    login.ProviderKey,
                    login.ProviderDisplayName,
                    UserId = user.Id
                });
            }
        }

        public async Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey) {
            const string command = "DELETE " +
                                   "FROM dbo.UserLogins " +
                                   "WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey AND UserId = @UserId;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                await sqlConnection.ExecuteAsync(command, new {
                    LoginProvider = loginProvider,
                    ProviderKey = providerKey,
                    UserId = user.Id
                });
            }
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user) {
            const string command = "SELECT * " +
                                   "FROM dbo.UserLogins " +
                                   "WHERE UserId = @UserId;";

            IEnumerable<UserLogin> userLogins = new List<UserLogin>();

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                userLogins = await sqlConnection.QueryAsync<UserLogin>(command, new {
                    UserId = user.Id
                });
            }

            return userLogins.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName)).ToList();
        }

        public async Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey) {
            string[] command =
            {
                "SELECT UserId " +
                "FROM dbo.UserLogins " +
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
