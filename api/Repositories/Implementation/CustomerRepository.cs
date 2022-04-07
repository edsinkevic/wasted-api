using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Wasted.Database.Interfaces;
using Wasted.Interfaces;
using WastedApi.Database;
using WastedApi.Helpers;
using WastedApi.Models;
using WastedApi.Requests;

namespace Wasted.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IWastedContext _context;

    public CustomerRepository(IWastedContext ctx)
    {
        _context = ctx;
    }
    public async Task<Either<List<string>, Customer>> Create(CustomerSignup req)
    {
        var errors = req.isValid();

        if (errors.Count > 0)
            return errors;

        var existing = await _context.Customers.Where(user => user.UserName == req.UserName).ToListAsync();
        if (existing.Count > 0)
            return new List<string> { "Username taken" };

        existing = await _context.Customers.Where(user => user.Email == req.Email).ToListAsync();
        if (existing.Count > 0)
            return new List<string> { "Email taken" };

        var salt = BCrypt.Net.BCrypt.GenerateSalt(13);
        var hash = BCrypt.Net.BCrypt.HashPassword(req.Password, salt);

        var member = new Customer
        {
            Id = Guid.NewGuid(),
            UserName = req.UserName,
            FirstName = req.FirstName,
            LastName = req.LastName,
            Email = req.Email,
            Hash = hash,
        };

        await _context.Customers.AddAsync(member);
        await _context.SaveChangesAsync();

        return member;

    }

    public async Task<Either<List<string>, Customer>> Get(UserLogin model)
    {
        var existing = _context.Customers.Include(x => x.Reservations).Where(user => user.UserName == model.UserName);
        if (existing.Count() == 0)
            return new List<string> { "User by that username was not found" };

        var user = await existing.FirstAsync();

        if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Hash))
            return new List<string> { "Incorrect password" };

        return user;
    }

    public async Task<Either<List<string>, Customer>> GetById(Guid id)
    {
        var existing = await _context.Customers
            .Include(x => x.Reservations)
            .ThenInclude(x => x.ReservationItems)
            .ThenInclude(x => x.Entry)
            .ThenInclude(x => x.Offer)
            .Where(x => x.Id == id).ToListAsync();

        if (existing.Count == 0)
            return new List<string> { "User not found" };

        return existing.First();
    }
}