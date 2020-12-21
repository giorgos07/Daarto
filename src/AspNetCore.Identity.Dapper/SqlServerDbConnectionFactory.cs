using System.Data;
using System.Data.SqlClient;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// Creates a new <see cref="SqlConnection"/> instance for connecting to Microsoft SQL Server.
    /// </summary>
    public class SqlServerDbConnectionStore : IDbConnectionStore
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

        private bool _disposed = false;
        /// <inheritdoc/>
        public void Dispose() {
            Dispose(true);
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">Indicates whether the method call comes from a Dispose method (its value is true) or from a finalizer (its value is false).</param>
        protected virtual void Dispose(bool disposing) {
            if (!_disposed && disposing) {
                // Free any other managed objects here.
                _sqlConnection?.Dispose();
                _disposed = true;
            }
        }
    }
}
