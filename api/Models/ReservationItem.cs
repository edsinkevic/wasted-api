using System;
using System.Collections.Generic;

namespace WastedApi.Models;
public partial class ReservationItem
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public Guid EntryId { get; set; }
    public int Amount { get; set; }

    public virtual OfferEntry Entry { get; set; } = null!;
    public virtual Reservation Reservation { get; set; } = null!;
}