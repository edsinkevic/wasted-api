using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace WastedApi.Requests
{
    public partial class OfferEntryUpdate
    {
        public Guid OfferEntryId { get; set; }
        public DateTime Expiry { get; set; }
        public int Amount { get; set; }

        public List<string> isValid()
        {
            var errors = new List<string>();

            if (Amount <= 0)
                errors.Append("Amount cannot be negative or zero!");

            if (Expiry < DateTime.Now)
                errors.Append("Expiration date would already pass!");

            return errors;
        }
    }
}
