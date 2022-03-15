using System;
using System.Collections.Generic;

namespace WastedApi.Models
{
    public partial class Jwt
    {
        public Guid Id { get; set; }
        public Guid Identity { get; set; }
        public string Jwt1 { get; set; } = null!;
        public DateTime Expiry { get; set; }
        public DateTime? LastTouched { get; set; }

        public virtual User IdentityNavigation { get; set; } = null!;
    }
}
