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

        public Category Category { get; set; }


        [JsonIgnore]
        public virtual Vendor AddedByNavigation { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<OfferEntry> OfferEntries { get; set; }
    }
}
