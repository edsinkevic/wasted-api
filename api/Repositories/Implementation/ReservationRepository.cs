
using System.Collections;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Wasted.Database.Interfaces;
using Wasted.Interfaces;
using WastedApi;
using WastedApi.Database;
using WastedApi.Extensions;
using WastedApi.Models;
using WastedApi.Requests;

namespace Wasted.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly IWastedContext _ctx;

    public ReservationRepository(IWastedContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<Either<List<string>, Reservation>> CompleteReservation(string code)
    {
        var existing = await _ctx.Reservations.Include(x => x.ReservationItems).Where(x => x.Code == code).ToListAsync();

        if (existing.Count() == 0)
            return new List<string> { "Reservation not found!" };

        var reservation = existing.First();

        await _ctx.Reservations.Include(x => x.ReservationItems).Where(x => x.Code == code).DeleteFromQueryAsync();

        return reservation;
    }

    public async Task<Either<List<string>, Reservation>> Create(ReservationCreate req)
    {
        var errors = req.isValid();

        if (errors.Count > 0)
            return errors;

        var entryIds = req.Items.Map(item => item.EntryId);

        var existingEntries = await _ctx.OfferEntries.Where(item => entryIds.Contains(item.Id)).ToListAsync();

        var itemMap = existingEntries.ToDictionary(item => item.Id);

        var desynchronizedItems = req.Items.Filter(item => item.Amount > itemMap[item.EntryId].Amount);

        if (desynchronizedItems.Count() > 0)
            return desynchronizedItems.Map(item => item.EntryId.ToString()).ToList();

        var reservedMap = req.Items.ToDictionary(entry => entry.EntryId);
        await _ctx.OfferEntries.BulkUpdateAsync(existingEntries.Map(entry =>
         new OfferEntry
         {
             Id = entry.Id,
             Amount = entry.Amount - reservedMap[entry.Id].Amount,
             Expiry = entry.Expiry,
             Added = entry.Added,
             OfferId = entry.OfferId
         }));

        var reservationId = Guid.NewGuid();
        var reservation = new Reservation
        {
            Id = reservationId,
            CustomerId = req.CustomerId,
            CreatedDate = DateTime.UtcNow.ToUtc(),
            ExpirationDate = DateTime.UtcNow.AddHours(1).ToUtc(),
            Code = String.Concat(Guid.NewGuid().ToString().Take(6)),
            ReservationItems = req.Items.Map(item =>
                new ReservationItem
                {
                    Id = Guid.NewGuid(),
                    ReservationId = reservationId,
                    EntryId = item.EntryId,
                    Amount = item.Amount,
                }).ToList()
        };

        var newReservation = await _ctx.Reservations.AddAsync(reservation);
        await _ctx.SaveChangesAsync();

        return newReservation.Entity;

    }

    public async Task<Either<List<string>, Reservation>> GetByCustomerId(string id)
    {
        Guid parsedId;
        if (!Guid.TryParse(id, out parsedId))
            return new List<string> { "Incorrect id format!" };

        var reservations = await _ctx.Reservations
            .Include(res => res.ReservationItems)
            .ThenInclude(res => res.Entry)
            .ThenInclude(res => res.Offer)
            .Where(res => res.CustomerId == parsedId)
            .AsNoTracking().ToListAsync();

        if (reservations.Count() < 1)
            return new List<string> { "Reservation not found!" };

        return reservations[0];
    }
}