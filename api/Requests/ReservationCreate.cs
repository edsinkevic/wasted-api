using WastedApi.Models;

namespace WastedApi.Requests;
public class ReservationCreate
{
    public Guid CustomerId { get; set; }

    public List<ReservationItemCreate> Items { get; set; } = null!;

    public List<string> isValid()
    {
        if (Items.Count < 1)
            return new List<string> { "Cannot initiate empty reservation!" };

        if (Items.Exists(item => item.Amount <= 0 || item.Amount > item.Entry.Amount))
            return new List<string> { "Invalid amounts" };

        return new List<string>();
    }

}