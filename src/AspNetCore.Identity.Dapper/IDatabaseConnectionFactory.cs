using System.Data;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Dapper
{
    internal interface IDatabaseConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }
}
