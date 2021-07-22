using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;
using SqlKata;

namespace AspNetCore.Identity.Dapper.Tables
{
    /// <summary>
    /// The default implementation of <see cref="IUserRolesTable{TRole,TKey,TUserRole}"/>.
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
            var query = new Query("AspNetRoles as r")
                .Join("AspNetUserRoles as ur", "ur.RoleId", "r.Id")
                .Where("ur.UserId", userId);
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var userRoles = await dbConnection.QueryAsync<TRole>(CompileQuery(query));
            return userRoles;
        }

        /// <inheritdoc/>
        public virtual async Task<TUserRole> FindUserRoleAsync(TKey userId, TKey roleId) {
            var query = new Query("AspNetUserRoles")
                .Where("userId", userId)
                .Where("RoleId", roleId);
            using var dbConnection =await DbConnectionFactory.CreateAsync();
            var userRole = await dbConnection.QuerySingleOrDefaultAsync<TUserRole>(CompileQuery(query));
            return userRole;
        }
    }
}
