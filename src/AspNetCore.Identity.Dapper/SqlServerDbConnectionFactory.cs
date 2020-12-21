using System.Data;
using System.Data.SqlClient;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// Creates a new <see cref="SqlConnection"/> instance for connecting to Microsoft SQL Server.
    /// </summary>
    public class SqlServerDbConnectionFactory : IDbConnectionFactory
    {
        /// <summary>
        /// The connection string to use for connecting to Microsoft SQL Server.
        /// </summary>
        public string ConnectionString { get; set; }

        private IDbConnection _sqlConnection;
        /// <inheritdoc/>
        public IDbConnection GetOrCreateConnection() {
            // we could open the connection here and it will remain open until explicitly closed, but if we return a closed connection (as below),
            // dapper will open and close with each query or transaction
            return _sqlConnection ??= new SqlConnection(ConnectionString);
        }
    }
}
