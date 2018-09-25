using System.Collections.Generic;
using System.Threading.Tasks;
using Daarto.Models;

namespace Daarto.Abstractions
{
    public interface IUserRepository
    {
        Task<(int Count, IEnumerable<User> Users)> GetUsersAsync(int pageNumber, int pageSize, string searchTerm, string sortField, SortDirection sortDirection);
    }
}
