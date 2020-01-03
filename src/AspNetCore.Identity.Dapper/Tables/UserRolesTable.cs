using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// The default implementation of <see cref="IUserRolesTable{TRole, TKey, TUserRole}"/>.
    /// </summary>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
    public class UserRolesTable<TRole, TKey, TUserRole> :
        IdentityTable,
        IUserRolesTable<TRole, TKey, TUserRole>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserRole : IdentityUserRole<TKey>, new()
    {
        /// <summary>
        /// Creates a new instance of <see cref="UserRolesTable{TRole, TKey, TUserRole}"/>.
        /// </summary>
        /// <param name="dbConnectionFactory">A factory for creating instances of <see cref="IDbConnection"/>.</param>
        public UserRolesTable(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TRole>> GetRolesAsync(TKey userId) {
            const string sql = "SELECT [r].* " +
                               "FROM [dbo].[AspNetRoles] AS [r] " +
                               "INNER JOIN [dbo].[AspNetUserRoles] AS [ur] ON [ur].[RoleId] = [r].[Id] " +
                               "WHERE [ur].[UserId] = @UserId;";
            var userRoles = await DbConnection.QueryAsync<TRole>(sql, new { UserId = userId });
            return userRoles;
        }

        /// <inheritdoc/>
        public virtual async Task<TUserRole> FindUserRoleAsync(TKey userId, TKey roleId) {
            const string sql = "SELECT * " +
                               "FROM [dbo].[AspNetUserRoles] " +
                               "WHERE [UserId] = @UserId AND [RoleId] = @RoleId;";
            var userRole = await DbConnection.QuerySingleOrDefaultAsync<TUserRole>(sql, new {
                UserId = userId,
                RoleId = roleId
            });
            return userRole;
        }
    }
}
