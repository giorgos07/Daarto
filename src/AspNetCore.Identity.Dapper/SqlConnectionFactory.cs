using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Dapper
{
    internal class SqlConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString) => _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

        public async Task<IDbConnection> CreateConnectionAsync() {
            try {
                var sqlConnection = new SqlConnection(_connectionString);
                await sqlConnection.OpenAsync();
                return sqlConnection;
            } catch {
                throw;
            }
        }
    }
}
