using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Daarto.Services.Abstract
{
    public interface IDatabaseConnectionService : IDisposable
    {
        Task<SqlConnection> CreateConnectionAsync();
        SqlConnection CreateConnection();
    }
}
