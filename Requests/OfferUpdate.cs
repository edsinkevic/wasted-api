using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using WastedApi.Models;

namespace WastedApi.Requests
{
    public partial class OfferUpdate
    {
        public Guid OfferId { get; set; }
        public string Name { get; set; } = null!;
        public int Weight { get; set; }
        public Category Category { get; set; }
        public Single Price { get; set; }

        public List<string> isValid()
        {
            var errors = new List<string>();

            if (Name == "")
                errors.Append("Name cannot be empty");

            if (Weight <= 0)
                errors.Append("Weight cannot be negative or zero");

            if (Price <= 0)
                errors.Append("Price cannot be negative or zero");

            return errors;
        }
    }
}
