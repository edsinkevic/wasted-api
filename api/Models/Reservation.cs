using System;
using System.Collections.Generic;
using WastedApi.Models;

namespace WastedApi;
public partial class Reservation
{
    public Reservation()
    {
        ReservationItems = new HashSet<ReservationItem>();
    }

    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateOnly CreatedDate { get; set; }
    public int ExpirationDate { get; set; }
    public string Code { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
    public virtual ICollection<ReservationItem> ReservationItems { get; set; }
}
