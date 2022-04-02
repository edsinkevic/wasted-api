using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WastedApi.Models
{
    public partial class Vendor
    {
        public Vendor()
        {
            Members = new HashSet<Member>();
            Offers = new HashSet<Offer>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Member> Members { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
    }
}
