using System;
using System.Collections.Generic;
using NodaTime;
using WastedApi.Models;

namespace WastedApi.Models;
public partial class Reservation
{
    public Reservation()
    {
        ReservationItems = new List<ReservationItem>();
    }

    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string Code { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
    public virtual List<ReservationItem> ReservationItems { get; set; }
}
