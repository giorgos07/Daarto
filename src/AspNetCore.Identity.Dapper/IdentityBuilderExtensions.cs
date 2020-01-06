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
        public static IdentityBuilder AddDapperStores(this IdentityBuilder builder, Action<DbConnectionContextOptions> configureAction = null) =>
            builder.AddDapperStores<DbConnectionContext>(configureAction);

        /// <summary>
        /// Adds a Dapper implementation of ASP.NET Core Identity stores.
        /// </summary>
        /// <param name="builder">Helper functions for configuring identity services.</param>
        /// <param name="configureAction">Delegate for configuring options </param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddDapperStores<TDbConnectionContext>(this IdentityBuilder builder, Action<DbConnectionContextOptions> configureAction = null) where TDbConnectionContext : DbConnectionContext {
            AddStores(builder.Services, builder.UserType, builder.RoleType, typeof(TDbConnectionContext), configureAction);
            return builder;
        }

        private static void AddStores(IServiceCollection services, Type userType, Type roleType, Type dbConnectionContextType, Action<DbConnectionContextOptions> configureAction = null) {
            var identityUserType = FindGenericBaseType(userType, typeof(IdentityUser<>));
            if (identityUserType == null) {
                throw new InvalidOperationException($"Method {nameof(AddDapperStores)} can only be called with a user that derives from IdentityUser<TKey>.");
            }
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var dbConnectionContextOptions = new DbConnectionContextOptions {
                ConnectionString = configuration.GetConnectionString("DefaultConnection"),
                DbConnectionFactory = new SqlServerDbConnectionFactory()
            };
            configureAction?.Invoke(dbConnectionContextOptions);
            services.AddSingleton(dbConnectionContextOptions);
            var keyType = identityUserType.GenericTypeArguments[0];
            var dbConnectionContext = FindGenericBaseType(dbConnectionContextType, typeof(DbConnectionContext<,,,,,,,>));
            services.AddScoped(typeof(IDbConnectionFactory), x => {
                var dbConnectionFactoryInstance = (IDbConnectionFactory)Activator.CreateInstance(dbConnectionContextOptions.DbConnectionFactory.GetType());
                dbConnectionFactoryInstance.ConnectionString = dbConnectionContextOptions.ConnectionString;
                return dbConnectionFactoryInstance;
            });
            Type userStoreType;
            var userClaimType = dbConnectionContext.GenericTypeArguments[3];
            var userRoleType = dbConnectionContext.GenericTypeArguments[4];
            var userLoginType = dbConnectionContext.GenericTypeArguments[5];
            var roleClaimType = dbConnectionContext.GenericTypeArguments[6];
            var userTokenType = dbConnectionContext.GenericTypeArguments[7];
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
