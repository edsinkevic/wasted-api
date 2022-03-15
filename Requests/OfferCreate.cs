using System;
using System.Collections.Generic;

namespace WastedApi.Requests
{
    public partial class OfferCreate
    {
        public string Name { get; set; } = null!;
        public Guid AddedBy { get; set; }
        public int Weight { get; set; }
        public Category Category { get; set; }
    }
    public enum Category
    {
        groceries, drinks, meat, sweets, other
    }
}
//CREATE TYPE CATEGORY_ENUM AS ENUM ('groceries', 'drinks', 'meat', 'sweets', 'other');        
