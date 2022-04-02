using System;
using System.Collections.Generic;

namespace WastedApi.Requests
{
    public partial class MemberSignup : CustomerSignup
    {
        public Guid VendorId { get; set; }
    }
}
