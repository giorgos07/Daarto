using System;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

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
        public static IdentityBuilder AddDapperStores(this IdentityBuilder builder, string connectionString) {
            AddStores(builder.Services, builder.UserType, builder.RoleType, connectionString);
            return builder;
        }

        private static void AddStores(IServiceCollection services, Type userType, Type roleType, string connectionString) {
            var identityUserType = FindGenericBaseType(userType, typeof(IdentityUser<>));
            if (identityUserType == null) {
                throw new InvalidOperationException($"Method {nameof(AddDapperStores)} can only be called with a user that derives from IdentityUser<TKey>.");
            }
            var keyType = identityUserType.GenericTypeArguments[0];
            if (roleType != null) {
                var identityRoleType = FindGenericBaseType(roleType, typeof(IdentityRole<>));
                if (identityRoleType == null) {
                    throw new InvalidOperationException($"Method {nameof(AddDapperStores)} can only be called with a role that derives from IdentityRole<TKey>.");
                }
            }
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
