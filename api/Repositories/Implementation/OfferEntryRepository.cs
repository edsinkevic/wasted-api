using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Wasted.Interfaces;
using WastedApi;
using WastedApi.Database;
using WastedApi.Extensions;
using WastedApi.Models;
using WastedApi.Requests;

namespace Wasted.Repositories;

public class OfferEntryRepository : IOfferEntryRepository
{
    private readonly WastedContext _ctx;

    public OfferEntryRepository(WastedContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<int> Clean() =>
        await _ctx.Database.ExecuteSqlRawAsync("DELETE FROM offer_entries WHERE expiry < now();");

    public async Task<Either<List<string>, OfferEntry>> Create(OfferEntryCreate req)
    {
        var errors = req.isValid();

        if (errors.Count > 0)
            return errors;

        var existing = _ctx.OfferEntries.Where(item => item.OfferId == req.OfferId && item.Expiry == req.Expiry);
        if (existing.Count() > 0)
        {
            var updated = await existing.FirstAsync();
            updated.Amount = updated.Amount + req.Amount;
            await _ctx.SaveChangesAsync();
            return updated;
        }

        var item = new OfferEntry
        {
            Amount = req.Amount,
            OfferId = req.OfferId,
            Added = DateOnly.FromDateTime(DateTime.Now),
            Expiry = req.Expiry,
            Id = Guid.NewGuid()
        };

        await _ctx.OfferEntries.AddAsync(item);
        await _ctx.SaveChangesAsync();
        return item;
    }

    public async Task<IEnumerable<OfferEntry>> Get() =>
        await _ctx.OfferEntries.Include(item => item.Offer).ThenInclude(offer => offer.Vendor).ToListAsync();

    public async Task<Either<List<string>, OfferEntry>> Update(OfferEntryUpdate req)
    {
        var errors = req.isValid();
        if (errors.Count > 0)
            return errors;

        var entry = await _ctx.OfferEntries.FindAsync(req.OfferEntryId);

        if (entry == null)
            return new List<string> { "Entry not found!" };

        entry.Amount = req.Amount;
        entry.Expiry = req.Expiry;

        await _ctx.SaveChangesAsync();

        return entry;
    }
}