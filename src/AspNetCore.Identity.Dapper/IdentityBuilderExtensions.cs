/*
 * The following code is inspired from https://github.com/aspnet/Identity/blob/master/src/EF/IdentityEntityFrameworkBuilderExtensions.cs
 */

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Reflection;

namespace AspNetCore.Identity.Dapper
{
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
            if (userType != typeof(ApplicationUser)) {
                throw new InvalidOperationException($"{nameof(AddDapperStores)} can only be called with a user that is of type {nameof(ApplicationUser)}.");
            }

            if (roleType != null) {
                if (roleType != typeof(ApplicationRole)) {
                    throw new InvalidOperationException($"{nameof(AddDapperStores)} can only be called with a role that is of type {nameof(ApplicationRole)}.");
                }

                services.TryAddScoped<IUserStore<ApplicationUser>, UserStore>();
                services.TryAddScoped<IRoleStore<ApplicationRole>, RoleStore>();
                services.TryAddScoped<IDatabaseConnectionFactory>(provider => new SqlConnectionFactory(connectionString));
            }
        }

        private static TypeInfo FindGenericBaseType(Type currentType, Type genericBaseType) {
            var type = currentType;

            while (type != null) {
                var typeInfo = type.GetTypeInfo();
                var genericType = type.IsGenericType ? type : null;

                if (genericType != null && genericType == genericBaseType) {
                    return typeInfo;
                }

                type = type.BaseType;
            }

            return null;
        }
    }
}
