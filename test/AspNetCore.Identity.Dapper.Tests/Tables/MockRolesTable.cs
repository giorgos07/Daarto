using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Tests
{
    public class MockRolesTable : IRolesTable<IdentityRole, string, IdentityRoleClaim<string>>
    {
        public static List<IdentityRole> Roles = new List<IdentityRole>();

        public Task<bool> CreateAsync(IdentityRole role) {
            Roles.Add(role);
            return Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync(string roleId) {
            var role = await FindByIdAsync(roleId);
            Roles.Remove(role);
            return true;
        }

        public Task<IdentityRole> FindByIdAsync(string roleId) {
            var role = Roles.SingleOrDefault(x => x.Id == roleId);
            return Task.FromResult(role);
        }

        public Task<IdentityRole> FindByNameAsync(string normalizedName) {
            var role = Roles.SingleOrDefault(x => x.NormalizedName == normalizedName);
            return Task.FromResult(role);
        }

        public async Task<bool> UpdateAsync(IdentityRole role, IList<IdentityRoleClaim<string>> claims = null) {
            var foundRole = await FindByIdAsync(role.Id);
            var roleIndex = Roles.IndexOf(role);
            Roles.ele
        }
    }
}
