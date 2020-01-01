using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// The default implementation of <see cref="IUserRolesTable{TKey, TUserRole}"/>.
    /// </summary>
    /// <typeparam name="TDbConnection">The type of the database connection class used to access the store.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    public class UserRolesTable<TDbConnection, TKey, TRole> : IUserRolesTable<TKey, TRole>
        where TDbConnection : IDbConnection
        where TKey : IEquatable<TKey>
        where TRole : IdentityRole<TKey>
    {
        /// <summary>
        /// Creates a new instance of <see cref="UserRolesTable{TDbConnection, TKey, TUserRole}"/>.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to use.</param>
        public UserRolesTable(TDbConnection dbConnection) {
            DbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }

        /// <summary>
        /// The <see cref="IDbConnection"/> to use.
        /// </summary>
        protected TDbConnection DbConnection { get; set; }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TRole>> GetRolesAsync(TKey userId) {
            const string sql = "SELECT [r].* " +
                               "FROM [dbo].[AspNetRoles] AS [r] " +
                               "INNER JOIN [dbo].[AspNetUserRoles] AS [ur] ON [ur].[RoleId] = [r].[Id] " +
                               "WHERE [ur].[UserId] = @UserId;";
            var userRoles = await DbConnection.QueryAsync<TRole>(sql, new { UserId = userId });
            return userRoles;
        }
    }
}
