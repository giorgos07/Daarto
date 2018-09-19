using System;

namespace AspNetCore.Identity.Dapper
{
    internal class UserLogin
    {
        public Guid UserId { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
    }
}
