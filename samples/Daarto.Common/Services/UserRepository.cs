using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Daarto.Abstractions;
using Daarto.Models;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Daarto.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserRepository(IDatabaseConnectionFactory databaseConnectionFactory) => _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));

        public async Task<(int Count, IEnumerable<User> Users)> GetUsersAsync(int pageNumber, int pageSize, string searchTerm, string sortField = "Email", SortDirection sortDirection = SortDirection.Ascending) {
            IEnumerable<User> users = new List<User>();
            int count;

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                var database = new QueryFactory(sqlConnection, new SqlServerCompiler());

                var query = database.Query("Users")
                                    .Select(nameof(User.Id), nameof(User.Email), nameof(User.EmailConfirmed), nameof(User.PhoneNumber), nameof(User.LockoutEnd), nameof(User.LockoutEnabled));

                if (!string.IsNullOrEmpty(searchTerm)) {
                    query = query.Where(q => q.Where(nameof(User.Email), "LIKE", $"%{searchTerm}%")
                                              .OrWhere(nameof(User.PhoneNumber), "LIKE", $"%{searchTerm}%"));
                }

                if (sortDirection == SortDirection.Ascending) {
                    query = query.OrderBy(sortField);
                } else {
                    query = query.OrderByDesc(sortField);
                }

                users = await query.ForPage(pageNumber, pageSize).GetAsync<User>();
                count = await database.Query("Users").CountAsync<int>();
            }

            return (count, users);
        }
    }
}
