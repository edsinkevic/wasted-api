using System;
using System.Collections.Generic;

namespace WastedApi.Models
{
    public partial class User
    {
        public User()
        {
            Jwts = new HashSet<Jwt>();
        }

        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Hash { get; set; } = null!;
        public string Role { get; set; } = null!;

        public virtual ICollection<Jwt> Jwts { get; set; }
    }
}
