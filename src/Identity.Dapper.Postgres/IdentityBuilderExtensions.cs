/*
 * The following code is inspired from https://github.com/aspnet/Identity/blob/master/src/EF/IdentityEntityFrameworkBuilderExtensions.cs
 */

using System;
using Identity.Dapper.Postgres.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Identity.Dapper.Postgres
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
    }
}
