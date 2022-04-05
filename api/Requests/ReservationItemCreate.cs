using WastedApi.Models;

namespace WastedApi.Requests;
public class ReservationItemCreate
{
    public int Amount { get; set; }

    public OfferEntry Entry { get; set; } = null!;


}