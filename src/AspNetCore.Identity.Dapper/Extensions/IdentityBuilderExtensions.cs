using System;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
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
        /// <param name="connectionString">The database connection string.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddDapperStores(this IdentityBuilder builder, string connectionString) =>
            builder.AddDapperStores<DefaultDbConnectionContext>(connectionString);

        /// <summary>
        /// Adds a Dapper implementation of ASP.NET Core Identity stores.
        /// </summary>
        /// <param name="builder">Helper functions for configuring identity services.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddDapperStores<TDbConnectionContext>(this IdentityBuilder builder, string connectionString) where TDbConnectionContext : DbConnectionContext {
            AddStores(builder.Services, builder.UserType, builder.RoleType, typeof(TDbConnectionContext), connectionString);
            return builder;
        }

        private static void AddStores(IServiceCollection services, Type userType, Type roleType, Type dbConnectionContextType, string connectionString) {
            var identityUserType = FindGenericBaseType(userType, typeof(IdentityUser<>));
            if (identityUserType == null) {
                throw new InvalidOperationException($"Method {nameof(AddDapperStores)} can only be called with a user that derives from IdentityUser<TKey>.");
            }
            var identityRoleType = FindGenericBaseType(roleType, typeof(IdentityRole<>));
            if (identityRoleType == null) {
                throw new InvalidOperationException($"Method {nameof(AddDapperStores)} can only be called with a role that derives from IdentityRole<TKey>.");
            }
            var keyType = identityUserType.GenericTypeArguments[0];
            var dbConnectionContext = FindGenericBaseType(dbConnectionContextType, typeof(DbConnectionContext<,,,,,,,>));
            var userClaimType = dbConnectionContext.GenericTypeArguments[3];
            var userRoleType = dbConnectionContext.GenericTypeArguments[4];
            var userLoginType = dbConnectionContext.GenericTypeArguments[5];
            var userTokenType = dbConnectionContext.GenericTypeArguments[6];
            var roleClaimType = dbConnectionContext.GenericTypeArguments[7];
            var dbConnectionFactoryProperty = dbConnectionContext.GetProperty(nameof(DbConnectionContext.DbConnectionFactory));
            var dbConnectionFactoryType = dbConnectionFactoryProperty.GetType();
            var dbConnectionFactoryInstance = Activator.CreateInstance(dbConnectionFactoryType);
            dbConnectionFactoryProperty.SetValue(dbConnectionFactoryInstance, Convert.ChangeType(connectionString, dbConnectionFactoryProperty.PropertyType), null);
            services.TryAddScoped(typeof(IDbConnectionFactory), dbConnectionContextType);
            services.TryAddScoped(
                typeof(IUsersTable<,,,,,>).MakeGenericType(userType, keyType, userClaimType, userRoleType, userLoginType, userTokenType),
                typeof(UsersTable<,,,,,>).MakeGenericType(userType, keyType, userClaimType, userRoleType, userLoginType, userTokenType)
            );
            services.TryAddScoped(typeof(IRolesTable<,,>).MakeGenericType(roleType, keyType, roleClaimType), typeof(RolesTable<,,>).MakeGenericType(roleType, keyType, roleClaimType));
            services.TryAddScoped(typeof(IUserClaimsTable<,>).MakeGenericType(keyType, userClaimType), typeof(UserClaimsTable<,>).MakeGenericType(keyType, userClaimType));
            services.TryAddScoped(typeof(IUserRolesTable<,,>).MakeGenericType(roleType, keyType, userRoleType), typeof(UserRolesTable<,,>).MakeGenericType(roleType, keyType, userRoleType));
            services.TryAddScoped(typeof(IUserLoginsTable<,,>).MakeGenericType(userType, keyType, userLoginType), typeof(UserLoginsTable<,,>).MakeGenericType(userType, keyType, userLoginType));
            services.TryAddScoped(typeof(IUserTokensTable<,>).MakeGenericType(keyType, userTokenType), typeof(UserTokensTable<,>).MakeGenericType(keyType, userTokenType));
            services.TryAddScoped(typeof(IRoleClaimsTable<,>).MakeGenericType(keyType, roleClaimType), typeof(RoleClaimsTable<,>).MakeGenericType(keyType, roleClaimType));
            services.TryAddScoped(
                typeof(IUserStore<>).MakeGenericType(userType),
                typeof(UserStore<,,,,,,,>).MakeGenericType(userType, roleType, keyType, userClaimType, userRoleType, userLoginType, userTokenType, roleClaimType)
            );
            services.TryAddScoped(
                typeof(IRoleStore<>).MakeGenericType(roleType),
                typeof(RoleStore<,,,>).MakeGenericType(roleType, keyType, userRoleType, roleClaimType)
            );
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
