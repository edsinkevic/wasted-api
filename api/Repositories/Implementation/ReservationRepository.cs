
using System.Collections;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Wasted.Interfaces;
using WastedApi;
using WastedApi.Database;
using WastedApi.Extensions;
using WastedApi.Models;
using WastedApi.Requests;

namespace Wasted.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly WastedContext _ctx;

    public ReservationRepository(WastedContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<Either<List<string>, Reservation>> CompleteReservation(string code)
    {
        var existing = await _ctx.Reservations.Include(x => x.ReservationItems).Where(x => x.Code == code).ToListAsync();

        if (existing.Count() == 0)
            return new List<string> { "Reservation not found!" };

        var reservation = existing.First();

        await _ctx.Database.ExecuteSqlRawAsync("ALTER TABLE reservation_items DISABLE TRIGGER ALL;");
        var toDelete = _ctx.Reservations.Where(x => x.Code == code);
        _ctx.Reservations.RemoveRange(toDelete);
        await _ctx.SaveChangesAsync();
        await _ctx.Database.ExecuteSqlRawAsync("ALTER TABLE reservation_items ENABLE TRIGGER ALL;");

        return reservation;
    }

    public async Task<Either<List<string>, Reservation>> CancelReservation(string id)
    {
        Guid parsedId;
        if (!Guid.TryParse(id, out parsedId))
            return new List<string> { "Id incorrect format!" };

        var existing = await _ctx.Reservations.Include(x => x.ReservationItems).Where(x => x.Id == parsedId).ToListAsync();

        if (existing.Count() == 0)
            return new List<string> { "Reservation not found!" };

        var reservation = existing.First();

        await _ctx.Database.ExecuteSqlRawAsync($"DELETE FROM reservations WHERE id = '{parsedId.ToString()}';");

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


        existingEntries.ForEach(entry =>
        {
            _ctx.Database
                .ExecuteSqlRaw($"UPDATE offer_entries SET amount = {entry.Amount - reservedMap[entry.Id].Amount} WHERE id = '{entry.Id.ToString()}';");
        }
        );

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

        var res = await _ctx.Reservations
        .Include(x => x.ReservationItems)
        .ThenInclude(x => x.Entry)
        .ThenInclude(x => x.Offer)
        .Where(x => x.Id == newReservation.Entity.Id)
        .ToListAsync();

        return res.First();
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