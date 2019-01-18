using System;
using System.Data;
using Npgsql;
using System.Threading.Tasks;
using D = Dapper;

namespace Identity.Dapper.Postgres
{
    internal class SqlConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString) => _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

        public async Task<IDbConnection> CreateConnectionAsync() 
        {
            var sqlConnection = new NpgsqlConnection(_connectionString);
            D.DefaultTypeMap.MatchNamesWithUnderscores = true;
            await sqlConnection.OpenAsync();
            return sqlConnection;
        }
    }
}
