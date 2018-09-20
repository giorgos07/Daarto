using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    internal class RoleStore : IQueryableRoleStore<ApplicationRole>
    {
        private readonly RolesTable _rolesTable;

        public RoleStore(IDatabaseConnectionFactory databaseConnectionFactory) => _rolesTable = new RolesTable(databaseConnectionFactory);

        public IQueryable<ApplicationRole> Roles => Task.Run(() => _rolesTable.GetAllRoles()).Result.AsQueryable();

        public Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            role.ThrowIfNull(nameof(role));
            return _rolesTable.CreateAsync(role, cancellationToken);
        }

        public Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            role.ThrowIfNull(nameof(role));
            return _rolesTable.UpdateAsync(role);
        }

        public Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            role.ThrowIfNull(nameof(role));
            return _rolesTable.DeleteAsync(role);
        }

        public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            role.ThrowIfNull(nameof(role));
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            role.ThrowIfNull(nameof(role));
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            role.ThrowIfNull(nameof(role));
            roleName.ThrowIfNull(nameof(roleName));
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            role.ThrowIfNull(nameof(role));
            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            role.ThrowIfNull(nameof(role));
            normalizedName.ThrowIfNull(nameof(normalizedName));
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            roleId.ThrowIfNull(nameof(roleId));
            var isValidGuid = Guid.TryParse(roleId, out var roleGuid);

            if (!isValidGuid) {
                throw new ArgumentException("Parameter roleId is not a valid Guid.", nameof(roleId));
            }

            return _rolesTable.FindByIdAsync(roleGuid);
        }

        public Task<ApplicationRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            normalizedRoleName.ThrowIfNull(nameof(normalizedRoleName));
            return _rolesTable.FindByNameAsync(normalizedRoleName);
        }

        public void Dispose() { }
    }
}
