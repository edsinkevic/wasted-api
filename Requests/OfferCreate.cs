using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using WastedApi.Models;

namespace WastedApi.Requests
{
    public partial class OfferCreate
    {
        public string Name { get; set; } = null!;
        public Guid AddedBy { get; set; }
        public int Weight { get; set; }
        public Category Category { get; set; }
    }
}
