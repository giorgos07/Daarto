using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;
using SqlKata;

namespace AspNetCore.Identity.Dapper.Tables
{
    /// <summary>
    /// The default implementation of <see cref="IUsersTable{TUser,TKey,TUserClaim,TUserRole,TUserLogin,TUserToken}"/>.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role and user.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
    /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    public class UsersTable<TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken> :
        IdentityTable,
        IUsersTable<TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        /// <summary>
        /// Creates a new instance of <see cref="UsersTable{TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken}"/>.
        /// </summary>
        /// <param name="dbConnectionFactory">A factory for creating instances of <see cref="IDbConnection"/>.</param>
        public UsersTable(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }

        /// <inheritdoc/>
        public virtual async Task<bool> CreateAsync(TUser user)
        {
            var query = new Query("AspNetUsers")
                .AsInsert(new
                {
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
                    user.AccessFailedCount,
                });
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var rowsInserted = await dbConnection.ExecuteAsync(CompileQuery(query));
            return rowsInserted == 1;
        }

        /// <inheritdoc/>
        public virtual async Task<bool> DeleteAsync(TKey userId)
        {
            var query = new Query("AspNetUsers")
                .Where("Id", userId)
                .AsDelete();
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var rowsDeleted = await dbConnection.ExecuteAsync(CompileQuery(query));
            return rowsDeleted == 1;
        }

        /// <inheritdoc/>
        public virtual async Task<TUser> FindByIdAsync(TKey userId)
        {
            var query = new Query("AspNetUsers")
                .Where("Id", userId);
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var user = await dbConnection.QuerySingleOrDefaultAsync<TUser>(CompileQuery(query));
            return user;
        }

        /// <inheritdoc/>
        public virtual async Task<TUser> FindByNameAsync(string normalizedUserName)
        {
            var query = new Query("AspNetUsers")
                .Where("NormalizedUserName", normalizedUserName);
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var user = await dbConnection.QuerySingleOrDefaultAsync<TUser>(CompileQuery(query));
            return user;
        }

        /// <inheritdoc/>
        public virtual async Task<TUser> FindByEmailAsync(string normalizedEmail)
        {
            var query = new Query("AspNetUsers")
                .Where("NormalizedEmail", normalizedEmail);
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var user = await dbConnection.QuerySingleOrDefaultAsync<TUser>(CompileQuery(query));
            return user;
        }

        /// <inheritdoc/>
        public virtual Task<bool> UpdateAsync(TUser user, IList<TUserClaim> claims, IList<TUserLogin> logins,
            IList<TUserToken> tokens)
        {
            return UpdateAsync(user, claims, null, logins, tokens);
        }

        /// <inheritdoc/>
        public virtual async Task<bool> UpdateAsync(TUser user, IList<TUserClaim> claims, IList<TUserRole> roles,
            IList<TUserLogin> logins, IList<TUserToken> tokens)
        {
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            using var transaction = dbConnection.BeginTransaction();
            
            await UpdateUserInfoAsync(user,dbConnection, transaction);
            
            await UpdateClaimsAsync(user, claims, dbConnection,transaction);

            await UpdateRolesAsync(user, roles, dbConnection,transaction);

            await UpdateLoginsAsync(user, logins, dbConnection,transaction);

            await UpdateTokensAsync(user, tokens, dbConnection,transaction);

            try
            {
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Update the user information 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="dbConnection"></param>
        /// <param name="transaction"></param>
        protected virtual async Task UpdateUserInfoAsync(TUser user, IDbConnection dbConnection,IDbTransaction transaction)
        {
            var updateUserSqlQuery = new Query("AspNetUsers")
                .Where("Id", user.Id)
                .AsUpdate(new
                {
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
                });
            await dbConnection.ExecuteAsync(CompileQuery(updateUserSqlQuery), transaction);
        }

        /// <summary>
        /// update user token table 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="tokens"></param>
        /// <param name="dbConnection"></param>
        /// <param name="transaction"></param>
        protected virtual async Task UpdateTokensAsync(TUser user, IList<TUserToken> tokens,IDbConnection dbConnection ,IDbTransaction transaction)
        {
            if (tokens?.Count() > 0)
            {
                var deleteTokensSqlQuery = new Query("AspNetUserTokens")
                    .Where("UserId", user.Id)
                    .AsDelete();
                await dbConnection.ExecuteAsync(CompileQuery(deleteTokensSqlQuery), transaction);

                var insertTokensSqlQuery = new Query("AspNetUserTokens")
                    .AsInsert(
                        new[]
                        {
                            "UserId",
                            "LoginProvider",
                            "Name",
                            "Value"
                        },
                        tokens.Select(x => new object[]
                        {
                            x.UserId,
                            x.LoginProvider,
                            x.Name,
                            x.Value
                        }).ToArray());

                await dbConnection.ExecuteAsync(CompileQuery(insertTokensSqlQuery), transaction);
            }
        }

        /// <summary>
        /// Update  logins table
        /// </summary>
        /// <param name="user"></param>
        /// <param name="logins"></param>
        /// <param name="dbConnection"></param>
        /// <param name="transaction"></param>
        protected virtual async Task UpdateLoginsAsync(TUser user, IList<TUserLogin> logins,IDbConnection dbConnection, IDbTransaction transaction)
        {
            if (logins?.Count() > 0)
            {
                var deleteLoginSqlQuery = new Query("AspNetUserLogins")
                    .Where("UserId", user.Id)
                    .AsDelete();

                await dbConnection.ExecuteAsync(CompileQuery(deleteLoginSqlQuery), transaction);

                var insertLoginSqlQuery = new Query("AspNetUserLogins")
                    .AsInsert(
                        new[]
                        {
                            "LoginProvider",
                            "ProviderKey",
                            "ProviderDisplayName",
                            "UserId"
                        },
                        logins.Select(x => new object[]
                        {
                            x.LoginProvider,
                            x.ProviderKey,
                            x.ProviderDisplayName,
                            user.Id
                        }).ToArray());
                await dbConnection.ExecuteAsync(CompileQuery(insertLoginSqlQuery), transaction);
            }
        }

        /// <summary>
        /// Update  user roles table
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roles"></param>
        /// <param name="dbConnection"></param>
        /// <param name="transaction"></param>
        protected virtual async Task UpdateRolesAsync(TUser user, IList<TUserRole> roles,IDbConnection dbConnection, IDbTransaction transaction)
        {
            if (roles?.Count() > 0)
            {
                var deleteRolesSqlQuery = new Query("AspNetUserRoles")
                    .Where("UserId", user.Id).AsDelete();
                await dbConnection.ExecuteAsync(CompileQuery(deleteRolesSqlQuery), transaction);
                var insertRoles = new Query("AspNetUserRoles")
                    .AsInsert(
                        new[]
                        {
                            "UserId",
                            "RoleId",
                        },
                        roles.Select(x => new object[]
                        {
                            user.Id,
                            x.RoleId,
                        }).ToArray());

                await dbConnection.ExecuteAsync(CompileQuery(insertRoles), transaction);
            }
        }

        /// <summary>
        /// update user claims
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claims"></param>
        /// <param name="dbConnection"></param>
        /// <param name="transaction"></param>
        protected virtual async Task UpdateClaimsAsync(TUser user, IList<TUserClaim> claims,IDbConnection dbConnection, IDbTransaction transaction)
        {
            if (claims?.Count() > 0)
            {
                var deleteClaimsSqlQuery = new Query("AspNetUserClaims")
                    .Where("UserId", user.Id).AsDelete();
                await dbConnection.ExecuteAsync(CompileQuery(deleteClaimsSqlQuery), transaction);
                var insertClaimsSqlQuery = new Query("AspNetUserClaims")
                    .AsInsert(
                        new[]
                        {
                            "UserId",
                            "ClaimType",
                            "ClaimValue"
                        },
                        claims.Select(x => new object[]
                        {
                            user.Id,
                            x.ClaimType,
                            x.ClaimValue
                        }).ToArray());
                await dbConnection.ExecuteAsync(CompileQuery(insertClaimsSqlQuery), transaction);
            }
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TUser>> GetUsersInRoleAsync(string roleName)
        {
            var query = new Query("AspNetUsers as u")
                .Join("AspNetUserRoles as ur", "u.Id", "ur.UserId")
                .Join("AspNetRoles as r", "ur.RoleId", "r.Id")
                .Where("r.Name", roleName);
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var users = await dbConnection.QueryAsync<TUser>(CompileQuery(query));
            return users;
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TUser>> GetUsersForClaimAsync(Claim claim)
        {
            var query = new Query("AspNetUsers as u")
                .Join("AspNetUserClaims as uc", "u.Id", "uc.UserId")
                .Where("uc.ClaimType", claim.Type)
                .Where("uc.ClaimValue", claim.Value);
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var users = await dbConnection.QueryAsync<TUser>(CompileQuery(query));
            return users;
        }
        
        
        
    }
}