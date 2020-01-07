using System;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// Extension methods on <see cref="IdentityBuilder"/> class.
    /// </summary>
    public static class IdentityBuilderExtensions
    {
        /// <summary>
        /// Adds a Dapper implementation of ASP.NET Core Identity stores.
        /// </summary>
        /// <param name="builder">Helper functions for configuring identity services.</param>
        /// <param name="configureAction">Delegate for configuring options </param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddDapperStores(this IdentityBuilder builder, Action<DapperStoreOptions> configureAction = null) {
            AddStores(builder.Services, builder.UserType, builder.RoleType, configureAction);
            return builder;
        }

        private static void AddStores(IServiceCollection services, Type userType, Type roleType, Action<DapperStoreOptions> configureAction = null) {
            var identityUserType = FindGenericBaseType(userType, typeof(IdentityUser<>));
            if (identityUserType == null) {
                throw new InvalidOperationException($"Method {nameof(AddDapperStores)} can only be called with a user that derives from IdentityUser<TKey>.");
            }
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var dbConnectionContextOptions = new DapperStoreOptions {
                ConnectionString = configuration.GetConnectionString("DefaultConnection"),
                DbConnectionFactory = new SqlServerDbConnectionFactory(),
                Services = services
            };
            configureAction?.Invoke(dbConnectionContextOptions);
            dbConnectionContextOptions.Services = null;
            services.AddSingleton(dbConnectionContextOptions);
            var keyType = identityUserType.GenericTypeArguments[0];
            services.AddScoped(typeof(IDbConnectionFactory), x => {
                var dbConnectionFactoryInstance = (IDbConnectionFactory)Activator.CreateInstance(dbConnectionContextOptions.DbConnectionFactory.GetType());
                dbConnectionFactoryInstance.ConnectionString = dbConnectionContextOptions.ConnectionString;
                return dbConnectionFactoryInstance;
            });
            Type userStoreType;
            var userClaimType = typeof(IdentityUserClaim<>).MakeGenericType(keyType);
            var userRoleType = typeof(IdentityUserRole<>).MakeGenericType(keyType);
            var userLoginType = typeof(IdentityUserLogin<>).MakeGenericType(keyType);
            var roleClaimType = typeof(IdentityRoleClaim<>).MakeGenericType(keyType);
            var userTokenType = typeof(IdentityUserToken<>).MakeGenericType(keyType);
            if (roleType != null) {
                var identityRoleType = FindGenericBaseType(roleType, typeof(IdentityRole<>));
                if (identityRoleType == null) {
                    throw new InvalidOperationException($"Method {nameof(AddDapperStores)} can only be called with a role that derives from IdentityRole<TKey>.");
                }
                services.AddScoped(
                    typeof(IUsersTable<,,,,,>).MakeGenericType(userType, keyType, userClaimType, userRoleType, userLoginType, userTokenType),
                    typeof(UsersTable<,,,,,>).MakeGenericType(userType, keyType, userClaimType, userRoleType, userLoginType, userTokenType)
                );
                services.AddScoped(typeof(IRolesTable<,,>).MakeGenericType(roleType, keyType, roleClaimType), typeof(RolesTable<,,>).MakeGenericType(roleType, keyType, roleClaimType));
                services.AddScoped(typeof(IUserRolesTable<,,>).MakeGenericType(roleType, keyType, userRoleType), typeof(UserRolesTable<,,>).MakeGenericType(roleType, keyType, userRoleType));
                services.AddScoped(typeof(IRoleClaimsTable<,>).MakeGenericType(keyType, roleClaimType), typeof(RoleClaimsTable<,>).MakeGenericType(keyType, roleClaimType));
                services.TryAddScoped(typeof(IRoleStore<>).MakeGenericType(roleType), typeof(RoleStore<,,,>).MakeGenericType(roleType, keyType, userRoleType, roleClaimType));
                userStoreType = typeof(UserStore<,,,,,,,>).MakeGenericType(userType, roleType, keyType, userClaimType, userRoleType, userLoginType, userTokenType, roleClaimType);
            } else {
                services.AddScoped(
                    typeof(IUsersOnlyTable<,,,,>).MakeGenericType(userType, keyType, userClaimType, userLoginType, userTokenType),
                    typeof(UsersTable<,,,,,>).MakeGenericType(userType, keyType, userClaimType, typeof(IdentityRole), userLoginType, userTokenType)
                );
                userStoreType = typeof(UserOnlyStore<,,,,>).MakeGenericType(userType, keyType, userClaimType, userLoginType, userTokenType);
            }
            services.AddScoped(typeof(IUserClaimsTable<,>).MakeGenericType(keyType, userClaimType), typeof(UserClaimsTable<,>).MakeGenericType(keyType, userClaimType));
            services.AddScoped(typeof(IUserLoginsTable<,,>).MakeGenericType(userType, keyType, userLoginType), typeof(UserLoginsTable<,,>).MakeGenericType(userType, keyType, userLoginType));
            services.AddScoped(typeof(IUserTokensTable<,>).MakeGenericType(keyType, userTokenType), typeof(UserTokensTable<,>).MakeGenericType(keyType, userTokenType));
            services.TryAddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
        }

        #region Table Extensions
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
        /// Add a custom implementation for <see cref="UserClaimsTable{TKey, TUserClaim}"/>.
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
        /// Add a custom implementation for <see cref="UserClaimsTable{TKey, TUserClaim}"/>.
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
        #endregion

        private static TypeInfo FindGenericBaseType(Type currentType, Type genericBaseType) {
            var type = currentType;
            while (type != null) {
                var typeInfo = type.GetTypeInfo();
                var genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
                if (genericType != null && genericType == genericBaseType) {
                    return typeInfo;
                }
                type = type.BaseType;
            }
            return null;
        }
    }
}
