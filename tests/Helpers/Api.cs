using Microsoft.Extensions.Logging;
using Moq;
using Wasted.Repositories;
using WastedApi.Controllers;
using WastedApi.Helpers;
using Microsoft.EntityFrameworkCore;
using WastedApi.Database;

namespace Wasted.Tests.Api;
public class Api
{
    private readonly WastedContext _ctx;
    private readonly JwtService _jwt;
    private readonly VendorRepository _vendors;
    private readonly OfferRepository _offers;
    private readonly OfferEntryRepository _entries;
    private readonly CustomerRepository _customers;
    private readonly MemberRepository _members;
    private readonly ReservationRepository _reservations;

    public VendorController Vendors { get; }
    public OfferController Offers { get; }
    public OfferEntryController OfferEntries { get; }
    public AuthenticationController Auth { get; }
    public ReservationController Reservations { get; }

    public void Clean()
    {
        _ctx.Database.ExecuteSqlRaw("TRUNCATE TABLE offer_entries CASCADE ;");
        _ctx.Database.ExecuteSqlRaw("TRUNCATE TABLE reservation_items CASCADE ;");
        _ctx.Database.ExecuteSqlRaw("TRUNCATE TABLE offers CASCADE ;");
        _ctx.Database.ExecuteSqlRaw("TRUNCATE TABLE vendors CASCADE;");
        _ctx.Database.ExecuteSqlRaw("TRUNCATE TABLE reservations CASCADE ;");
        _ctx.Database.ExecuteSqlRaw("TRUNCATE TABLE customers CASCADE ;");
        _ctx.Database.ExecuteSqlRaw("TRUNCATE TABLE members CASCADE ;");
    }

    public Api(WastedContext ctx)
    {
        _ctx = ctx;
        _jwt = new JwtService();
        _vendors = new VendorRepository(_ctx);
        _offers = new OfferRepository(_ctx);
        _entries = new OfferEntryRepository(_ctx);
        _customers = new CustomerRepository(_ctx);
        _members = new MemberRepository(_ctx);
        _reservations = new ReservationRepository(_ctx);

        var vl = new Mock<ILogger<VendorController>>();
        var ol = new Mock<ILogger<OfferController>>();
        var el = new Mock<ILogger<OfferEntryController>>();
        var al = new Mock<ILogger<AuthenticationController>>();
        var rl = new Mock<ILogger<ReservationController>>();

        Vendors = new VendorController(vl.Object, _vendors);
        Offers = new OfferController(ol.Object, _offers);
        OfferEntries = new OfferEntryController(el.Object, _entries);
        Auth = new AuthenticationController(al.Object, _jwt, _members, _customers);
        Reservations = new ReservationController(rl.Object, _reservations);

        Clean();
    }
}