using System;

namespace Identity.Dapper.Postgres.Stores
{
    internal class UserRole
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
