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

        /// <inheritdoc/>
        public IDbConnection Create() {
            var sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            return sqlConnection;
        }
    }
}
