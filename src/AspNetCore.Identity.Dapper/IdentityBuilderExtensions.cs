using System;
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

        private static void AddStores(IServiceCollection services, Type userType, Type roleType, string connectionString) { }
    }
}
