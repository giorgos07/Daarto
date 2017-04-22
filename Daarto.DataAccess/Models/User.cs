using System;

namespace Daarto.DataAccess.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTime? LockoutEndDateTimeUtc { get; set; }
    }
}