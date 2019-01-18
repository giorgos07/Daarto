using System.Data;
using System.Threading.Tasks;

namespace Identity.Dapper.Postgres
{
    internal interface IDatabaseConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }
}
