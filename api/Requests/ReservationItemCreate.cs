using WastedApi.Models;

namespace WastedApi.Requests;
public class ReservationItemCreate
{
    public int Amount { get; set; }

    public Guid EntryId { get; set; }
    public int EntryAmount { get; set; }


}