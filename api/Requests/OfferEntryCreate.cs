using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace WastedApi.Requests
{
    public partial class OfferEntryCreate
    {
        public DateOnly Expiry { get; set; }
        public Guid OfferId { get; set; }
        public int Amount { get; set; }

        public List<string> isValid()
        {
            var errors = new List<string>();

            if (Amount <= 0)
                errors.Append("Amount cannot be negative or zero");

            if (Expiry < DateOnly.FromDateTime(DateTime.Now))
                errors.Append("Expiration date has already passed!");

            return errors;
        }
    }
}
