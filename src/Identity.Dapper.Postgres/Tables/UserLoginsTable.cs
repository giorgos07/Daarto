using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Identity.Dapper.Postgres.Stores;
using Microsoft.AspNetCore.Identity;

namespace Identity.Dapper.Postgres.Tables
{
    internal class UserLoginsTable
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserLoginsTable(IDatabaseConnectionFactory databaseConnectionFactory) => _databaseConnectionFactory = databaseConnectionFactory;

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user) {
            const string command = "SELECT * " +
                                   "FROM identity_user_logins " +
                                   "WHERE user_id = @UserId;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                return (
                    await sqlConnection.QueryAsync<UserLogin>(command, new { UserId = user.Id })
                )
                .Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName))
                .ToList(); ;
            }
        }

        public async Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey) {
            string[] command =
            {
                "SELECT user_id " +
                "FROM identity_user_logins " +
                "WHERE login_provider = @LoginProvider AND provider_key = @ProviderKey;"
            };

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                var userId = await sqlConnection.QuerySingleOrDefaultAsync<Guid?>(command[0], new {
                    LoginProvider = loginProvider,
                    ProviderKey = providerKey
                });

                if (userId == null) {
                    return null;
                }

                command[0] = "SELECT * " +
                             "FROM identity_users " +
                             "WHERE id = @Id;";

                return await sqlConnection.QuerySingleAsync<ApplicationUser>(command[0], new { Id = userId });
            }
        }
    }
}
