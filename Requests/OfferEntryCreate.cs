using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace WastedApi.Requests
{
    public partial class OfferEntryCreate
    {
        public DateTime Expiry { get; set; }
        public Guid OfferId { get; set; }
        public int Amount { get; set; }
    }
}
