using Daarto.IdentityProvider.Entities;
using Dapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Daarto.IdentityProvider.Tables
{
    public class UsersTable
    {
        private SqlConnection _sqlConnection;

        public UsersTable(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            const string command = "INSERT INTO dbo.Users " +
                                   "VALUES (@Id, @FirstName, @LastName, @UserName, @NormalizedUserName, @Email, @NormalizedEmail, @EmailConfirmed, " +
                                           "@PasswordHash, @PhoneNumber, @PhoneNumberConfirmed, @PhotoUrl, @Address, @ConcurrencyStamp, @SecurityStamp, " +
                                           "@RegistrationDate, @LastLoginDate, @LockoutEnabled, @LockoutEndDateTimeUtc, @TwoFactorEnabled, @AccessFailedCount);";

            int rowsInserted = Task.Run(() => _sqlConnection.ExecuteAsync(command, new
            {
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
            }), cancellationToken).Result;

            return Task.FromResult(rowsInserted.Equals(1) ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
            {
                Code = string.Empty,
                Description = $"The user with email {user.Email} could not be inserted in the dbo.Users table."
            }));
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            const string command = "DELETE " +
                                   "FROM dbo.Users " +
                                   "WHERE Id = @Id;";

            int rowsDeleted = Task.Run(() => _sqlConnection.ExecuteAsync(command, new
            {
                user.Id
            }), cancellationToken).Result;

            return Task.FromResult(rowsDeleted.Equals(1) ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
            {
                Code = string.Empty,
                Description = $"The user with email {user.Email} could not be deleted from the dbo.Users table."
            }));
        }

        public Task<ApplicationUser> FindByIdAsync(Guid userId)
        {
            const string command = "SELECT * " +
                                   "FROM dbo.Users " +
                                   "WHERE Id = @Id;";

            return _sqlConnection.QuerySingleOrDefaultAsync<ApplicationUser>(command, new
            {
                Id = userId
            });
        }

        public Task<ApplicationUser> FindByNameAsync(string normalizedUserName)
        {
            const string command = "SELECT * " +
                                   "FROM dbo.Users " +
                                   "WHERE NormalizedUserName = @NormalizedUserName;";

            return _sqlConnection.QuerySingleOrDefaultAsync<ApplicationUser>(command, new
            {
                NormalizedUserName = normalizedUserName
            });
        }

        public Task<ApplicationUser> FindByEmailAsync(string normalizedEmail)
        {
            const string command = "SELECT * " +
                                   "FROM dbo.Users " +
                                   "WHERE NormalizedEmail = @NormalizedEmail;";

            return _sqlConnection.QuerySingleOrDefaultAsync<ApplicationUser>(command, new
            {
                NormalizedEmail = normalizedEmail
            });
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            const string command = "UPDATE dbo.Users " +
                                   "SET FirstName = @FirstName, LastName = @LastName, UserName = @UserName, NormalizedUserName = @NormalizedUserName, Email = @Email, NormalizedEmail = @NormalizedEmail, " +
                                       "EmailConfirmed = @EmailConfirmed, PasswordHash = @PasswordHash, PhoneNumber = @PhoneNumber, PhoneNumberConfirmed = @PhoneNumberConfirmed, PhotoUrl = @PhotoUrl, Address = @Address, " +
                                       "ConcurrencyStamp = @ConcurrencyStamp, SecurityStamp = @SecurityStamp, RegistrationDate = @RegistrationDate, LastLoginDate = @LastLoginDate, LockoutEnabled = @LockoutEnabled, LockoutEndDateTimeUtc = @LockoutEndDateTimeUtc, " +
                                       "TwoFactorEnabled = @TwoFactorEnabled, AccessFailedCount = @AccessFailedCount " +
                                   "WHERE Id = @Id;";

            int rowsUpdated = Task.Run(() => _sqlConnection.ExecuteAsync(command, new
            {
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
            }), cancellationToken).Result;

            return Task.FromResult(rowsUpdated.Equals(1) ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
            {
                Code = string.Empty,
                Description = $"The user with email {user.Email} could not be updated in the dbo.Users table."
            }));
        }

        public Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            const string command = "SELECT * " +
                                   "FROM dbo.Users;";

            return _sqlConnection.QueryAsync<ApplicationUser>(command);
        }

        public void Dispose()
        {
            if (_sqlConnection == null)
            {
                return;
            }

            _sqlConnection.Dispose();
            _sqlConnection = null;
        }
    }
}