using System;
using System.Collections.Generic;

namespace WastedApi.Models
{
    public partial class Member
    {
        public Guid Id { get; set; }
        public Guid VendorId { get; set; }
        public string UserName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Hash { get; set; } = null!;
        public virtual Vendor Vendor { get; set; } = null!;
    }
}
