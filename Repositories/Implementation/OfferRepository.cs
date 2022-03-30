using Wasted.Interfaces;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using WastedApi.Database;
using WastedApi.Models;
using WastedApi.Requests;

namespace Wasted.Repositories;

public class OfferRepository : IOfferRepository
{
    private readonly WastedContext _ctx;

    public OfferRepository(WastedContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<IEnumerable<Offer>> Get() =>
        await _ctx.Offers.ToListAsync();

    public async Task<IEnumerable<Offer>> GetByVendorName(string name) =>
        await _ctx.Offers
            .Include(item => item.Vendor)
            .Where(item => item.Vendor.Name == name).ToListAsync();
    public async Task<Either<List<string>, Offer>> Create(OfferCreate req)
    {
        var errors = req.isValid();

        if (errors.Count > 0)
            return errors;

        var existing = await _ctx.Offers.Where(offer => offer.Name == req.Name && offer.Weight == req.Weight).ToListAsync();
        if (existing.Count > 0)
            return new List<string> { "Offer with same name and weight already exists!" };

        var offer = new Offer
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Weight = req.Weight,
            AddedBy = req.AddedBy,
            Category = req.Category,
            Price = req.Price
        };

        await _ctx.Offers.AddAsync(offer);
        await _ctx.SaveChangesAsync();
        return offer;
    }

    public async Task<Either<List<string>, Offer>> Update(OfferUpdate req)
    {
        var errors = req.isValid();
        if (errors.Count > 0)
            return errors;

        var offer = await _ctx.Offers.FindAsync(req.OfferId);

        if (offer == null)
            return new List<string> { "Entry not found!" };

        offer.Name = req.Name;
        offer.Weight = req.Weight;
        offer.Category = req.Category;
        offer.Price = req.Price;

        await _ctx.SaveChangesAsync();

        return offer;
    }
}