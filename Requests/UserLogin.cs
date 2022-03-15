using System;
using System.Collections.Generic;

namespace WastedApi.Requests
{
    public partial class UserLogin
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
