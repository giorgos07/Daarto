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

            IEnumerable<UserClaim> userClaims = new List<UserClaim>();

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                userClaims = await sqlConnection.QueryAsync<UserClaim>(command, new {
                    UserId = user.Id
                });
            }

            return userClaims.Select(e => new Claim(e.ClaimType, e.ClaimValue)).ToList();
        }
    }
}
