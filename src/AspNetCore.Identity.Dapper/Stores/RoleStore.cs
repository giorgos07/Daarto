using Daarto.Services.Abstract;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Dapper
{
    internal class RoleStore : IQueryableRoleStore<ApplicationRole>
    {
        private readonly RolesTable _rolesTable;

        public RoleStore(IDatabaseConnectionService databaseConnection)
        {
            _rolesTable = new RolesTable(databaseConnection.CreateConnection());
        }

        public IQueryable<ApplicationRole> Roles => Task.Run(() => _rolesTable.GetAllRoles()).Result.AsQueryable();

        public Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "Parameter role is not set to an instance of an object.");
            }

            return _rolesTable.CreateAsync(role, cancellationToken);
        }

        public Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "Parameter role is not set to an instance of an object.");
            }

            return _rolesTable.UpdateAsync(role, cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "Parameter role is not set to an instance of an object.");
            }
            
            return _rolesTable.DeleteAsync(role, cancellationToken);
        }

        public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "Parameter role is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "Parameter role is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "Parameter role is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName), "Parameter roleName cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            role.Name = roleName;
            return Task.FromResult<object>(null);
        }

        public Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "Parameter role is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "Parameter role is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(normalizedName))
            {
                throw new ArgumentNullException(nameof(normalizedName), "Parameter normalizedName cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            role.NormalizedName = normalizedName;
            return Task.FromResult<object>(null);
        }

        public Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentNullException(nameof(roleId), "Parameter roleId cannot be null or empty.");
            }

            bool isValidGuid = Guid.TryParse(roleId, out Guid roleGuid);

            if (!isValidGuid)
            {
                throw new ArgumentException("Parameter roleId is not a valid Guid.", nameof(roleId));
            }

            cancellationToken.ThrowIfCancellationRequested();
            return _rolesTable.FindByIdAsync(roleGuid);
        }

        public Task<ApplicationRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(normalizedRoleName))
            {
                throw new ArgumentNullException(nameof(normalizedRoleName), "Parameter normalizedRoleName cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return _rolesTable.FindByNameAsync(normalizedRoleName);
        }

        public void Dispose()
        {
            _rolesTable.Dispose();
        }
    }
}