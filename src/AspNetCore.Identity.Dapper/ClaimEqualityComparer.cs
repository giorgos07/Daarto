using System.Collections.Generic;
using System.Security.Claims;

namespace AspNetCore.Identity.Dapper
{
    internal class ClaimEqualityComparer : IEqualityComparer<Claim>
    {
        public bool Equals(Claim x, Claim y) => x.Type == y.Type && x.Value == y.Value;

        public int GetHashCode(Claim obj) => obj.GetHashCode();
    }
}
