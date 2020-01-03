using System;
using System.Data;
using System.Data.SqlClient;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// Creates a new <see cref="SqlConnection"/> instance for connection to Microsoft SQL Server.
    /// </summary>
    public class SqlServerDbConnection : IDbConnectionFactory
    {
        /// <inheritdoc/>
        public IDbConnection Create() {
            throw new NotImplementedException();
        }
    }
}
