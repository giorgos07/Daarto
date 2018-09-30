using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
                                   "VALUES (@Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail, @EmailConfirmed, @PasswordHash, @SecurityStamp, @ConcurrencyStamp, " +
                                           "@PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled, @LockoutEnd, @LockoutEnabled, @AccessFailedCount);";

            int rowsInserted;

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                rowsInserted = await sqlConnection.ExecuteAsync(command, new {
                    user.Id,
                    user.UserName,
                    user.NormalizedUserName,
                    user.Email,
                    user.NormalizedEmail,
                    user.EmailConfirmed,
                    user.PasswordHash,
                    user.SecurityStamp,
                    user.ConcurrencyStamp,
                    user.PhoneNumber,
                    user.PhoneNumberConfirmed,
                    user.TwoFactorEnabled,
                    user.LockoutEnd,
                    user.LockoutEnabled,
                    user.AccessFailedCount
                });
            }

            return rowsInserted == 1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = nameof(CreateAsync),
                Description = $"User with email {user.Email} could not be inserted."
            });
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user) {
            const string command = "DELETE " +
                                   "FROM dbo.Users " +
                                   "WHERE Id = @Id;";

            int rowsDeleted;

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                rowsDeleted = await sqlConnection.ExecuteAsync(command, new {
                    user.Id
                });
            }

            return rowsDeleted == 1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = nameof(DeleteAsync),
                Description = $"User with email {user.Email} could not be deleted."
            });
        }

        public async Task<ApplicationUser> FindByIdAsync(Guid userId) {
            const string command = "SELECT * " +
                                   "FROM dbo.Users " +
                                   "WHERE Id = @Id;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                return await sqlConnection.QuerySingleOrDefaultAsync<ApplicationUser>(command, new {
                    Id = userId
                });
            }
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName) {
            const string command = "SELECT * " +
                                   "FROM dbo.Users " +
                                   "WHERE NormalizedUserName = @NormalizedUserName;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                return await sqlConnection.QuerySingleOrDefaultAsync<ApplicationUser>(command, new {
                    NormalizedUserName = normalizedUserName
                });
            }
        }

        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail) {
            const string command = "SELECT * " +
                                   "FROM dbo.Users " +
                                   "WHERE NormalizedEmail = @NormalizedEmail;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                return await sqlConnection.QuerySingleOrDefaultAsync<ApplicationUser>(command, new {
                    NormalizedEmail = normalizedEmail
                });
            }
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user) {
            // The implementation here might look a little strange, however there is a reason for this.
            // ASP.NET Core Identity stores follow a UOW (Unit of Work) pattern which practically means that when an operation is called it does not necessarily writes to the database.
            // It tracks the changes made and finally commits to the database. UserStore methods just manipulate the user and only CreateAsync, UpdateAsync and DeleteAsync of IUserStore<>
            // write to the database. This makes sense because this way we avoid connection to the database all the time and also we can commit all changes at once by using a transaction.
            const string updateUserCommand =
                "UPDATE dbo.Users " +
                "SET UserName = @UserName, NormalizedUserName = @NormalizedUserName, Email = @Email, NormalizedEmail = @NormalizedEmail, EmailConfirmed = @EmailConfirmed, " +
                    "PasswordHash = @PasswordHash, SecurityStamp = @SecurityStamp, ConcurrencyStamp = @ConcurrencyStamp, PhoneNumber = @PhoneNumber, " +
                    "PhoneNumberConfirmed = @PhoneNumberConfirmed, TwoFactorEnabled = @TwoFactorEnabled, LockoutEnd = @LockoutEnd, LockoutEnabled = @LockoutEnabled, " +
                    "AccessFailedCount = @AccessFailedCount " +
                "WHERE Id = @Id;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                using (var transaction = sqlConnection.BeginTransaction()) {
                    await sqlConnection.ExecuteAsync(updateUserCommand, new {
                        user.UserName,
                        user.NormalizedUserName,
                        user.Email,
                        user.NormalizedEmail,
                        user.EmailConfirmed,
                        user.PasswordHash,
                        user.SecurityStamp,
                        user.ConcurrencyStamp,
                        user.PhoneNumber,
                        user.PhoneNumberConfirmed,
                        user.TwoFactorEnabled,
                        user.LockoutEnd,
                        user.LockoutEnabled,
                        user.AccessFailedCount,
                        user.Id
                    }, transaction);

                    if (user.Claims.Count() > 0) {
                        const string deleteClaimsCommand = "DELETE " +
                                                           "FROM dbo.UserClaims " +
                                                           "WHERE UserId = @UserId;";

                        await sqlConnection.ExecuteAsync(deleteClaimsCommand, new {
                            UserId = user.Id
                        }, transaction);

                        const string insertClaimsCommand = "INSERT INTO dbo.UserClaims (UserId, ClaimType, ClaimValue) " +
                                                           "VALUES (@UserId, @ClaimType, @ClaimValue);";

                        await sqlConnection.ExecuteAsync(insertClaimsCommand, user.Claims.Select(x => new {
                            UserId = user.Id,
                            ClaimType = x.Type,
                            ClaimValue = x.Value
                        }), transaction);
                    }

                    if (user.Logins.Count() > 0) {
                        const string deleteLoginsCommand = "DELETE " +
                                                           "FROM dbo.UserLogins " +
                                                           "WHERE UserId = @UserId;";

                        await sqlConnection.ExecuteAsync(deleteLoginsCommand, new {
                            UserId = user.Id
                        }, transaction);

                        const string insertLoginsCommand = "INSERT INTO dbo.UserLogins (LoginProvider, ProviderKey, ProviderDisplayName, UserId) " +
                                                           "VALUES (@LoginProvider, @ProviderKey, @ProviderDisplayName, @UserId);";

                        await sqlConnection.ExecuteAsync(insertLoginsCommand, user.Logins.Select(x => new {
                            x.LoginProvider,
                            x.ProviderKey,
                            x.ProviderDisplayName,
                            UserId = user.Id
                        }), transaction);
                    }

                    if (user.Roles.Count() > 0) {
                        const string deleteRolesCommand = "DELETE " +
                                                          "FROM dbo.UserRoles " +
                                                          "WHERE UserId = @UserId;";

                        await sqlConnection.ExecuteAsync(deleteRolesCommand, new {
                            UserId = user.Id
                        }, transaction);

                        const string insertRolesCommand = "INSERT INTO dbo.UserRoles (UserId, RoleId) " +
                                                          "VALUES (@UserId, @RoleId);";

                        await sqlConnection.ExecuteAsync(insertRolesCommand, user.Roles.Select(x => new {
                            UserId = user.Id,
                            x.RoleId
                        }), transaction);
                    }

                    if (user.Tokens.Count() > 0) {
                        const string deleteTokensCommand = "DELETE " +
                                                           "FROM dbo.UserTokens " +
                                                           "WHERE UserId = @UserId;";

                        await sqlConnection.ExecuteAsync(deleteTokensCommand, new {
                            UserId = user.Id
                        }, transaction);

                        const string insertTokensCommand = "INSERT INTO dbo.UserTokens (UserId, LoginProvider, Name, Value) " +
                                                           "VALUES (@UserId, @LoginProvider, @Name, @Value);";

                        await sqlConnection.ExecuteAsync(insertTokensCommand, user.Tokens.Select(x => new {
                            x.UserId,
                            x.LoginProvider,
                            x.Name,
                            x.Value
                        }), transaction);
                    }

                    try {
                        transaction.Commit();
                    } catch {
                        try {
                            transaction.Rollback();
                        } catch {
                            return IdentityResult.Failed(new IdentityError {
                                Code = nameof(UpdateAsync),
                                Description = $"User with email {user.Email} could not be updated. Operation could not be rolled back."
                            });
                        }

                        return IdentityResult.Failed(new IdentityError {
                            Code = nameof(UpdateAsync),
                            Description = $"User with email {user.Email} could not be updated. Operation was rolled back."
                        });
                    }
                }
            }

            return IdentityResult.Success;
        }

        public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName) {
            const string command = "SELECT * " +
                                   "FROM dbo.Users AS u " +
                                   "INNER JOIN dbo.UserRoles AS ur ON u.Id = ur.UserId " +
                                   "INNER JOIN dbo.Roles AS r ON ur.RoleId = r.Id " +
                                   "WHERE r.Name = @RoleName;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                return (await sqlConnection.QueryAsync<ApplicationUser>(command, new {
                    RoleName = roleName
                })).ToList();
            }
        }

        public async Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim) {
            const string command = "SELECT * " +
                                   "FROM dbo.Users AS u " +
                                   "INNER JOIN dbo.UserClaims AS uc ON u.Id = uc.UserId " +
                                   "WHERE uc.ClaimType = @ClaimType AND uc.ClaimValue = @ClaimValue;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                return (await sqlConnection.QueryAsync<ApplicationUser>(command, new {
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                })).ToList();
            }
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers() {
            const string command = "SELECT * " +
                                   "FROM dbo.Users;";

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                return await sqlConnection.QueryAsync<ApplicationUser>(command);
            }
        }
    }
}
