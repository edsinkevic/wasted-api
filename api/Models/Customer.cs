using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WastedApi.Models
{
    public partial class Customer : IUser
    {
        public List<Reservation> Reservations { get; set; } = null!;
    }
}
