using Daarto.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Daarto.DataAccess.Abstract
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsersAsync(int pageNumber, int pageSize, int sortExpression, SortDirection sortDirection, string searchPhrase);
        int GetTotalNumberOfUsers();
    }
}