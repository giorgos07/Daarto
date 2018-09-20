using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;

namespace AspNetCore.Identity.Dapper
{
    internal class UsersClaimsTable
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UsersClaimsTable(IDatabaseConnectionFactory databaseConnectionFactory) => _databaseConnectionFactory = databaseConnectionFactory;

        public async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user) {
            const string command = "SELECT * " +
                                   "FROM dbo.UsersClaims " +
                                   "WHERE UserId = @UserId;";

            IEnumerable<UserClaim> userClaims = new List<UserClaim>();

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                userClaims = await sqlConnection.QueryAsync<UserClaim>(command, new {
                    UserId = user.Id
                });
            }

            return userClaims.Select(e => new Claim(e.ClaimType, e.ClaimValue)).ToList();
        }

        public async Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims) {
            const string command = "INSERT INTO dbo.UsersClaims " +
                                   "VALUES (@Id, @UserId, @ClaimType, @ClaimValue);";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                await sqlConnection.ExecuteAsync(command, claims.Select(e => new {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    ClaimType = e.Type,
                    ClaimValue = e.Value
                }));
            }
        }

        public async Task ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim) {
            const string command = "UPDATE dbo.UsersClaims " +
                                   "SET ClaimType = @NewClaimType, ClaimValue = @NewClaimValue " +
                                   "WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimType;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                await sqlConnection.ExecuteAsync(command, new {
                    NewClaimType = newClaim.Type,
                    NewClaimValue = newClaim.Value,
                    UserId = user.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
            }
        }
    }
}
