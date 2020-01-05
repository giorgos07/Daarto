using System;
using System.Data;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// The default <see cref="DbConnectionContext"/> used by the library.
    /// </summary>
    public class DefaultDbConnectionContext : DbConnectionContext
    {
        /// <inheritdoc/>
        public override IDbConnectionFactory DbConnectionFactory => new SqlServerDbConnectionFactory();
    }

    /// <summary>
    /// Base class for the Dapper database context used for identity.
    /// </summary>
    public class DbConnectionContext : DbConnectionContext<IdentityUser, IdentityRole, string> { }

    /// <summary>
    /// Base class for the Dapper database context used for identity.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    public class DbConnectionContext<TUser> : DbConnectionContext<TUser, IdentityRole, string> where TUser : IdentityUser { }

    /// <summary>
    /// Base class for the Dapper database context used for identity.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role and user.</typeparam>
    public class DbConnectionContext<TUser, TRole, TKey> : DbConnectionContext<TUser, TRole, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityRoleClaim<TKey>, IdentityUserToken<TKey>>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    { }

    /// <summary>
    /// Base class for the Dapper database context used for identity.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role and user.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
    /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    /// <typeparam name="TRoleClaim">The type representing a role claim.</typeparam>
    public abstract class DbConnectionContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
    {
        /// <summary>
        /// A factory for creating instances of <see cref="IDbConnection"/>.
        /// </summary>
        public virtual IDbConnectionFactory DbConnectionFactory { get; set; }
    }
}
