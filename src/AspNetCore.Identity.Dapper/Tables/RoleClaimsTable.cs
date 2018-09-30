using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;

namespace AspNetCore.Identity.Dapper
{
    internal class RoleClaimsTable
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public RoleClaimsTable(IDatabaseConnectionFactory databaseConnectionFactory) => _databaseConnectionFactory = databaseConnectionFactory;

        public async Task<IList<Claim>> GetClaimsAsync(string roleId) {
            const string command = "SELECT * " +
                                   "FROM dbo.RoleClaims " +
                                   "WHERE RoleId = @RoleId;";

            IEnumerable<RoleClaim> roleClaims = new List<RoleClaim>();

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                return (
                    await sqlConnection.QueryAsync<RoleClaim>(command, new { RoleId = roleId })
                )
                .Select(x => new Claim(x.ClaimType, x.ClaimValue))
                .ToList();
            }
        }
    }
}
