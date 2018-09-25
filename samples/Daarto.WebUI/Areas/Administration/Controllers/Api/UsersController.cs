using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daarto.Abstractions;
using Daarto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Daarto.Controllers.Administration.Api
{
    [Area("Administration")]
    [Authorize(Roles = "Administrator")]
    [Route("api/users")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        public const string Name = "Users";
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository) => _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        [HttpPost]
        public async Task<ActionResult<User>> GetUsers(DataTable dataTable) {
            var pageNumber = dataTable.Start / dataTable.Length + 1;
            var order = dataTable.Order.FirstOrDefault();

            var sortDirection = order != null
                ? (order.Direction == "asc" ? SortDirection.Ascending : SortDirection.Descending)
                : SortDirection.Ascending;

            var orderColumnMapping = new Dictionary<int, string> {
                { 0, nameof(Models.User.Email) },
                { 1, nameof(Models.User.EmailConfirmed) },
                { 3, nameof(Models.User.LockoutEnabled) },
                { 4, nameof(Models.User.LockoutEnd) }
            };

            var (Count, Users) = await _userRepository.GetUsersAsync(pageNumber, dataTable.Length, string.IsNullOrEmpty(dataTable.Search.Value) ? string.Empty : dataTable.Search.Value,
                orderColumnMapping[order?.Column ?? 0], sortDirection);

            return Ok(new DataTableResponse<User> {
                Data = Users,
                RecordsFiltered = string.IsNullOrEmpty(dataTable.Search.Value) ? Count : Users.Count(),
                Draw = dataTable.Draw,
                RecordsTotal = Count
            });
        }
    }
}
