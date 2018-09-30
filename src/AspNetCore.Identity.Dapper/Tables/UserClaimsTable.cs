using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;

namespace AspNetCore.Identity.Dapper
{
    internal class UserClaimsTable
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserClaimsTable(IDatabaseConnectionFactory databaseConnectionFactory) => _databaseConnectionFactory = databaseConnectionFactory;

        public async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user) {
            const string command = "SELECT * " +
                                   "FROM dbo.UserClaims " +
                                   "WHERE UserId = @UserId;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                return (
                    await sqlConnection.QueryAsync<UserClaim>(command, new { UserId = user.Id })
                )
                .Select(e => new Claim(e.ClaimType, e.ClaimValue))
                .ToList(); ;
            }
        }
    }
}
