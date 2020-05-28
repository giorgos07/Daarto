using System;
using AspNetCore.Identity.Dapper;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions on <see cref="DapperStoreOptions"/> type.
    /// </summary>
    public static class DapperStoreOptionsExtensions
    {
        /// <summary>
        /// Add a custom implementation for <see cref="RoleClaimsTable{TKey, TRoleClaim}"/>.
        /// </summary>
        /// <typeparam name="TRoleClaimsTable">The type of the table to register.</typeparam>
        /// <typeparam name="TRoleClaim">The type of the class representing a role claim.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddRoleClaimsTable<TRoleClaimsTable, TRoleClaim>(this DapperStoreOptions options)
            where TRoleClaimsTable : RoleClaimsTable<string, TRoleClaim>
            where TRoleClaim : IdentityRoleClaim<string>, new() {
            options.AddRoleClaimsTable<TRoleClaimsTable, string, TRoleClaim>();
        }

        /// <summary>
        /// Add a custom implementation for <see cref="RoleClaimsTable{TKey, TRoleClaim}"/>.
        /// </summary>
        /// <typeparam name="TRoleClaimsTable">The type of the table to register.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
        /// <typeparam name="TRoleClaim">The type of the class representing a role claim.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddRoleClaimsTable<TRoleClaimsTable, TKey, TRoleClaim>(this DapperStoreOptions options)
            where TRoleClaimsTable : RoleClaimsTable<TKey, TRoleClaim>
            where TKey : IEquatable<TKey>
            where TRoleClaim : IdentityRoleClaim<TKey>, new() {
            options.Services.AddScoped(typeof(IRoleClaimsTable<,>).MakeGenericType(typeof(TKey), typeof(TRoleClaim)), typeof(TRoleClaimsTable));
        }

        /// <summary>
        /// Add a custom implementation for <see cref="RolesTable{TRole, TKey, TRoleClaim}"/>.
        /// </summary>
        /// <typeparam name="TRolesTable">The type of the table to register.</typeparam>
        /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddRolesTable<TRolesTable, TRole>(this DapperStoreOptions options)
            where TRolesTable : RolesTable<TRole, string, IdentityRoleClaim<string>>
            where TRole : IdentityRole<string> {
            options.AddRolesTable<TRolesTable, TRole, string, IdentityRoleClaim<string>>();
        }

        /// <summary>
        /// Add a custom implementation for <see cref="RolesTable{TRole, TKey, TRoleClaim}"/>.
        /// </summary>
        /// <typeparam name="TRolesTable">The type of the table to register.</typeparam>
        /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
        /// <typeparam name="TRoleClaim">The type of the class representing a role claim.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddRolesTable<TRolesTable, TRole, TKey, TRoleClaim>(this DapperStoreOptions options)
            where TRolesTable : RolesTable<TRole, TKey, TRoleClaim>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
            where TRoleClaim : IdentityRoleClaim<TKey>, new() {
            options.Services.AddScoped(typeof(IRolesTable<,,>).MakeGenericType(typeof(TRole), typeof(TKey), typeof(TRoleClaim)), typeof(TRolesTable));
        }

        /// <summary>
        /// Add a custom implementation for <see cref="UserClaimsTable{TKey, TUserClaim}"/>.
        /// </summary>
        /// <typeparam name="TUserClaimsTable">The type of the table to register.</typeparam>
        /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUserClaimsTable<TUserClaimsTable, TUserClaim>(this DapperStoreOptions options)
            where TUserClaimsTable : UserClaimsTable<string, TUserClaim>
            where TUserClaim : IdentityUserClaim<string>, new() {
            options.AddUserClaimsTable<TUserClaimsTable, string, TUserClaim>();
        }

        /// <summary>
        /// Add a custom implementation for <see cref="UserClaimsTable{TKey, TUserClaim}"/>.
        /// </summary>
        /// <typeparam name="TUserClaimsTable">The type of the table to register.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
        /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUserClaimsTable<TUserClaimsTable, TKey, TUserClaim>(this DapperStoreOptions options)
            where TUserClaimsTable : UserClaimsTable<TKey, TUserClaim>
            where TKey : IEquatable<TKey>
            where TUserClaim : IdentityUserClaim<TKey>, new() {
            options.Services.AddScoped(typeof(IUserClaimsTable<,>).MakeGenericType(typeof(TKey), typeof(TUserClaim)), typeof(TUserClaimsTable));
        }

        /// <summary>
        /// Add a custom implementation for <see cref="UserLoginsTable{TUser, TKey, TUserLogin}"/>.
        /// </summary>
        /// <typeparam name="TUserLoginsTable">The type of the table to register.</typeparam>
        /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUserLoginsTable<TUserLoginsTable, TUserLogin>(this DapperStoreOptions options)
            where TUserLoginsTable : UserLoginsTable<IdentityUser, string, TUserLogin>
            where TUserLogin : IdentityUserLogin<string>, new() {
            options.AddUserLoginsTable<TUserLoginsTable, IdentityUser, string, TUserLogin>();
        }

        /// <summary>
        /// Add a custom implementation for <see cref="UserLoginsTable{TUser, TKey, TUserLogin}"/>.
        /// </summary>
        /// <typeparam name="TUserLoginsTable">The type of the table to register.</typeparam>
        /// <typeparam name="TUser">The type representing a user.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
        /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUserLoginsTable<TUserLoginsTable, TUser, TKey, TUserLogin>(this DapperStoreOptions options)
            where TUserLoginsTable : UserLoginsTable<TUser, TKey, TUserLogin>
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
            where TUserLogin : IdentityUserLogin<TKey>, new() {
            options.Services.AddScoped(typeof(IUserLoginsTable<,,>).MakeGenericType(typeof(TUser), typeof(TKey), typeof(TUserLogin)), typeof(TUserLoginsTable));
        }

        /// <summary>
        /// Add a custom implementation for <see cref="UserRolesTable{TRole, TKey, TUserRole}"/>.
        /// </summary>
        /// <typeparam name="TUserRolesTable">The type of the table to register.</typeparam>
        /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUserRolesTable<TUserRolesTable, TUserRole>(this DapperStoreOptions options)
            where TUserRolesTable : UserRolesTable<IdentityRole, string, TUserRole>
            where TUserRole : IdentityUserRole<string>, new() {
            options.AddUserRolesTable<TUserRolesTable, IdentityRole, string, TUserRole>();
        }

        /// <summary>
        /// Add a custom implementation for <see cref="UserRolesTable{TRole, TKey, TUserRole}"/>.
        /// </summary>
        /// <typeparam name="TUserRolesTable">The type of the table to register.</typeparam>
        /// <typeparam name="TRole">The type representing a role.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
        /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUserRolesTable<TUserRolesTable, TRole, TKey, TUserRole>(this DapperStoreOptions options)
            where TUserRolesTable : UserRolesTable<TRole, TKey, TUserRole>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
            where TUserRole : IdentityUserRole<TKey>, new() {
            options.Services.AddScoped(typeof(IUserRolesTable<,,>).MakeGenericType(typeof(TRole), typeof(TKey), typeof(TUserRole)), typeof(TUserRolesTable));
        }

        /// <summary>
        /// Add a custom implementation for <see cref="UsersTable{TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken}"/>.
        /// </summary>
        /// <typeparam name="TUsersTable">The type of the table to register.</typeparam>
        /// <typeparam name="TUser">The type representing a user.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUsersTable<TUsersTable, TUser>(this DapperStoreOptions options)
            where TUsersTable : UsersTable<TUser, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>>
            where TUser : IdentityUser<string> {
            options.AddUsersTable<TUsersTable, TUser, string>();
        }

        /// <summary>
        /// Add a custom implementation for <see cref="UsersTable{TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken}"/>.
        /// </summary>
        /// <typeparam name="TUsersTable">The type of the table to register.</typeparam>
        /// <typeparam name="TUser">The type representing a user.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a role and user.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUsersTable<TUsersTable, TUser, TKey>(this DapperStoreOptions options)
            where TUsersTable : UsersTable<TUser, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityUserToken<TKey>>
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey> {
            options.AddUsersTable<TUsersTable, TUser, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityUserToken<TKey>>();
        }

        /// <summary>
        /// Add a custom implementation for <see cref="UsersTable{TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken}"/>.
        /// </summary>
        /// <typeparam name="TUsersTable">The type of the table to register.</typeparam>
        /// <typeparam name="TUser">The type representing a user.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a role and user.</typeparam>
        /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
        /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
        /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
        /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUsersTable<TUsersTable, TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken>(this DapperStoreOptions options)
            where TUsersTable : UsersTable<TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken>
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
            where TUserClaim : IdentityUserClaim<TKey>, new()
            where TUserRole : IdentityUserRole<TKey>, new()
            where TUserLogin : IdentityUserLogin<TKey>, new()
            where TUserToken : IdentityUserToken<TKey>, new() {
            options.Services.AddScoped(typeof(IUsersTable<,,,,,>).MakeGenericType(typeof(TUser), typeof(TKey), typeof(TUserClaim), typeof(TUserRole), typeof(TUserLogin), typeof(TUserToken)), typeof(TUsersTable));
        }

        /// <summary>
        /// Add a custom implementation for <see cref="UserTokensTable{TKey, TUserToken}"/>.
        /// </summary>
        /// <typeparam name="TUserTokensTable">The type of the table to register.</typeparam>
        /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUserTokensTable<TUserTokensTable, TUserToken>(this DapperStoreOptions options)
            where TUserTokensTable : UserTokensTable<string, TUserToken>
            where TUserToken : IdentityUserToken<string>, new() {
            options.AddUserTokensTable<TUserTokensTable, string, TUserToken>();
        }

        /// <summary>
        /// Add a custom implementation for <see cref="UserTokensTable{TKey, TUserToken}"/>.
        /// </summary>
        /// <typeparam name="TUserTokensTable">The type of the table to register.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a role and user.</typeparam>
        /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUserTokensTable<TUserTokensTable, TKey, TUserToken>(this DapperStoreOptions options)
            where TUserTokensTable : UserTokensTable<TKey, TUserToken>
            where TKey : IEquatable<TKey>
            where TUserToken : IdentityUserToken<TKey>, new() {
            options.Services.AddScoped(typeof(IUserTokensTable<,>).MakeGenericType(typeof(TKey), typeof(TUserToken)), typeof(TUserTokensTable));
        }
    }
}
