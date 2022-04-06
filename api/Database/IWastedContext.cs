using Microsoft.EntityFrameworkCore;
using WastedApi;
using WastedApi.Models;

namespace Wasted.Database.Interfaces;

public interface IWastedContext
{
    public DbSet<Member> Members { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<OfferEntry> OfferEntries { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<ReservationItem> ReservationItems { get; set; }
    public int SaveChanges();
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
}