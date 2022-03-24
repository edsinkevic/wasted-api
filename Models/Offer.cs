using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WastedApi.Requests;

namespace WastedApi.Models
{
    public partial class Offer
    {
        public Offer()
        {
            OfferEntries = new HashSet<OfferEntry>();
        }

        public Guid Id { get; set; }
        public Guid AddedBy { get; set; }
        public string Name { get; set; } = null!;
        public int Weight { get; set; }
        public double Price { get; set; }
        public Category Category { get; set; }
        public virtual Vendor Vendor { get; set; } = null!;
        public virtual ICollection<OfferEntry> OfferEntries { get; set; }
    }
}
