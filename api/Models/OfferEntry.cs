using System;
using System.Collections.Generic;
using WastedApi.Models;

namespace WastedApi.Models;
public partial class OfferEntry
{
    public Guid Id { get; set; }
    public Guid OfferId { get; set; }
    public DateTime Expiry { get; set; }
    public DateTime Added { get; set; }
    public int Amount { get; set; }

    public virtual Offer Offer { get; set; } = null!;

    public List<ReservationItem> ReservationItems { get; set; } = null!;

}
