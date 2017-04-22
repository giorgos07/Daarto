using Daarto.IdentityProvider.Entities;
using Dapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Daarto.IdentityProvider.Tables
{
    public class UsersLoginsTable
    {
        private readonly SqlConnection _sqlConnection;

        public UsersLoginsTable(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            const string command = "INSERT INTO dbo.UsersLogins " +
                                   "VALUES (@LoginProvider, @ProviderKey, @UserId, @ProviderDisplayName);";

            return _sqlConnection.ExecuteAsync(command, new
            {
                login.LoginProvider,
                login.ProviderKey,
                UserId = user.Id,
                login.ProviderDisplayName
            });
        }

        public Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey)
        {
            const string command = "DELETE " +
                                   "FROM dbo.UsersLogins " +
                                   "WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey;";

            return _sqlConnection.ExecuteAsync(command, new
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            });
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            const string command = "SELECT * " +
                                   "FROM dbo.UsersLogins " +
                                   "WHERE UserId = @UserId;";

            IEnumerable<UserLogin> userLogins = Task.Run(() => _sqlConnection.QueryAsync<UserLogin>(command, new
            {
                UserId = user.Id
            }), cancellationToken).Result;

            return Task.FromResult<IList<UserLoginInfo>>(userLogins.Select(e => new UserLoginInfo(e.LoginProvider, e.ProviderKey, e.ProviderDisplayName)).ToList());
        }

        public Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            string[] command =
            {
                "SELECT UserId " +
                "FROM dbo.UsersLogins " +
                "WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey;"
            };

            Guid? userGuid = Task.Run(() => _sqlConnection.QuerySingleOrDefaultAsync<Guid?>(command[0], new
            {
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            }), cancellationToken).Result;

            if (userGuid == null)
            {
                return Task.FromResult<ApplicationUser>(null);
            }

            command[0] = "SELECT * " +
                         "FROM dbo.Users " +
                         "WHERE Id = @Id;";

            return _sqlConnection.QuerySingleAsync<ApplicationUser>(command[0], new { Id = userGuid });
        }
    }
}