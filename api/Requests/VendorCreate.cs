using System;
using System.Collections.Generic;

namespace WastedApi.Requests
{
    public partial class VendorCreate
    {
        public string Name { get; set; } = null!;

        public List<string> isValid()
        {
            if (Name == "")
                return new List<string> { "Vendor name cannot be empty!" };

            return new List<string>();
        }
    }
}
