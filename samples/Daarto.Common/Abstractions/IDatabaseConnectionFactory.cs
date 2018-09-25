using System.Data;
using System.Threading.Tasks;

namespace Daarto.Abstractions
{
    public interface IDatabaseConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }
}
