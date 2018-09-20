using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    internal class UsersTable
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UsersTable(IDatabaseConnectionFactory databaseConnectionFactory) => _databaseConnectionFactory = databaseConnectionFactory;

        public async Task<IdentityResult> CreateAsync(ApplicationUser user) {
            const string command = "INSERT INTO dbo.Users " +
                                   "VALUES (@Id, @FirstName, @LastName, @UserName, @NormalizedUserName, @Email, @NormalizedEmail, @EmailConfirmed, " +
                                           "@PasswordHash, @PhoneNumber, @PhoneNumberConfirmed, @PhotoUrl, @Address, @ConcurrencyStamp, @SecurityStamp, " +
                                           "@RegistrationDate, @LastLoginDate, @LockoutEnabled, @LockoutEndDateTimeUtc, @TwoFactorEnabled, @AccessFailedCount);";

            var rowsInserted = 0;

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                rowsInserted = await sqlConnection.ExecuteAsync(command, new {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.UserName,
                    user.NormalizedUserName,
                    user.Email,
                    user.NormalizedEmail,
                    user.EmailConfirmed,
                    user.PasswordHash,
                    user.PhoneNumber,
                    user.PhoneNumberConfirmed,
                    user.PhotoUrl,
                    user.Address,
                    user.ConcurrencyStamp,
                    user.SecurityStamp,
                    user.RegistrationDate,
                    user.LastLoginDate,
                    user.LockoutEnabled,
                    user.LockoutEndDateTimeUtc,
                    user.TwoFactorEnabled,
                    user.AccessFailedCount
                });
            }

            return rowsInserted == 1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = string.Empty,
                Description = $"User with email {user.Email} could not be inserted in the database."
            });
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user) {
            const string command = "DELETE " +
                                   "FROM dbo.Users " +
                                   "WHERE Id = @Id;";

            var rowsDeleted = 0;

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                rowsDeleted = await sqlConnection.ExecuteAsync(command, new {
                    user.Id
                });
            }

            return rowsDeleted == 1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = string.Empty,
                Description = $"User with email {user.Email} could not be deleted from the database."
            });
        }

        public async Task<ApplicationUser> FindByIdAsync(Guid userId) {
            const string command = "SELECT * " +
                                   "FROM dbo.Users " +
                                   "WHERE Id = @Id;";

            ApplicationUser user;

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                user = await sqlConnection.QuerySingleOrDefaultAsync<ApplicationUser>(command, new {
                    Id = userId
                });
            }

            return user;
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName) {
            const string command = "SELECT * " +
                                   "FROM dbo.Users " +
                                   "WHERE NormalizedUserName = @NormalizedUserName;";

            ApplicationUser user;

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                user = await sqlConnection.QuerySingleOrDefaultAsync<ApplicationUser>(command, new {
                    NormalizedUserName = normalizedUserName
                });
            }

            return user;
        }

        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail) {
            const string command = "SELECT * " +
                                   "FROM dbo.Users " +
                                   "WHERE NormalizedEmail = @NormalizedEmail;";

            ApplicationUser user;

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                user = await sqlConnection.QuerySingleOrDefaultAsync<ApplicationUser>(command, new {
                    NormalizedEmail = normalizedEmail
                });
            }

            return user;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user) {
            const string command = "UPDATE dbo.Users " +
                                   "SET FirstName = @FirstName, LastName = @LastName, UserName = @UserName, NormalizedUserName = @NormalizedUserName, Email = @Email, NormalizedEmail = @NormalizedEmail, " +
                                       "EmailConfirmed = @EmailConfirmed, PasswordHash = @PasswordHash, PhoneNumber = @PhoneNumber, PhoneNumberConfirmed = @PhoneNumberConfirmed, PhotoUrl = @PhotoUrl, Address = @Address, " +
                                       "ConcurrencyStamp = @ConcurrencyStamp, SecurityStamp = @SecurityStamp, RegistrationDate = @RegistrationDate, LastLoginDate = @LastLoginDate, LockoutEnabled = @LockoutEnabled, LockoutEndDateTimeUtc = @LockoutEndDateTimeUtc, " +
                                       "TwoFactorEnabled = @TwoFactorEnabled, AccessFailedCount = @AccessFailedCount " +
                                   "WHERE Id = @Id;";

            var rowsUpdated = 0;

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                rowsUpdated = await sqlConnection.ExecuteAsync(command, new {
                    user.FirstName,
                    user.LastName,
                    user.UserName,
                    user.NormalizedUserName,
                    user.Email,
                    user.NormalizedEmail,
                    user.EmailConfirmed,
                    user.PasswordHash,
                    user.PhoneNumber,
                    user.PhoneNumberConfirmed,
                    user.PhotoUrl,
                    user.Address,
                    user.ConcurrencyStamp,
                    user.SecurityStamp,
                    user.RegistrationDate,
                    user.LastLoginDate,
                    user.LockoutEnabled,
                    user.LockoutEndDateTimeUtc,
                    user.TwoFactorEnabled,
                    user.AccessFailedCount,
                    user.Id
                });
            }

            return rowsUpdated == 1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = string.Empty,
                Description = $"User with email {user.Email} could not be updated."
            });
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers() {
            const string command = "SELECT * " +
                                   "FROM dbo.Users;";

            IEnumerable<ApplicationUser> users = new List<ApplicationUser>();

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                users = await sqlConnection.QueryAsync<ApplicationUser>(command);
            }

            return users;
        }
    }
}
