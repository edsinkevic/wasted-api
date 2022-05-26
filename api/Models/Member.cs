using System;
using System.Collections.Generic;

namespace WastedApi.Models
{
    public partial class Member : IUser
    {
        public Guid VendorId { get; set; }
        public virtual Vendor Vendor { get; set; } = null!;
        public virtual List<AdminRole> AdminRoles { get; set; } = null!;
    }
}
