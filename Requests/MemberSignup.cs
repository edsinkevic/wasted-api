using System;
using System.Collections.Generic;

namespace WastedApi.Requests
{
    public partial class MemberSignup : UserSignup
    {
        public Guid VendorId { get; set; }
    }
}
