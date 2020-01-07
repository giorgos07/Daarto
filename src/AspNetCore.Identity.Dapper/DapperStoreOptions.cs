using System.Data;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// Options for configuring Dapper stores.
    /// </summary>
    public class DapperStoreOptions
    {
        internal IServiceCollection Services;
        /// <summary>
        /// The connection string to use for connecting to the data source.
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// A factory for creating instances of <see cref="IDbConnection"/>.
        /// </summary>
        public IDbConnectionFactory DbConnectionFactory { get; set; }
    }
}
