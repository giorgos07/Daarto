using Daarto.DataAccess.Abstract;
using Daarto.DataAccess.Models;
using Daarto.Services.Abstract;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Daarto.DataAccess.Concrete
{
    public class UserRepository : IUserRepository
    {
        private readonly SqlConnection _sqlConnection;

        public UserRepository(IDatabaseConnectionService databaseConnection)
        {
            _sqlConnection = databaseConnection.CreateConnection();
        }

        public int GetTotalNumberOfUsers()
        {
            const string command = "SELECT * " +
                                   "FROM GetTotalNumberOfUsers;";

            return _sqlConnection.ExecuteScalar<int>(command);
        }

        public async Task<IEnumerable<User>> GetUsersAsync(int pageNumber, int pageSize, int sortExpression, SortDirection sortDirection, string searchPhrase)
        {
            IEnumerable<User> users = await _sqlConnection.QueryAsync<User>("GetsUsers", new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortExpression = sortExpression,
                SortDirection = sortDirection == SortDirection.Ascending ? "asc" : "desc",
                SearchPhrase = searchPhrase
            }, commandType: CommandType.StoredProcedure);

            return users;
        }
    }
}