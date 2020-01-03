using System.Data;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// A factory for creating instances of <see cref="IDbConnection"/>.
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Creates a new instance of the underlying <see cref="IDbConnection"/>.
        /// </summary>
        IDbConnection Create();
    }
}
