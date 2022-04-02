using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Wasted.Database.Interfaces;
using Wasted.Interfaces;
using WastedApi.Database;
using WastedApi.Helpers;
using WastedApi.Models;
using WastedApi.Requests;

namespace Wasted.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly IWastedContext _ctx;
    private readonly JwtService _jwt;

    public MemberRepository(IWastedContext ctx, JwtService jwt)
    {
        _ctx = ctx;
        _jwt = jwt;
    }
    public async Task<Either<List<string>, Member>> Create(MemberSignup req)
    {
        var errors = req.isValid();

        if (errors.Count > 0)
            return errors;

        var existing = await _ctx.Members.Where(user => user.UserName == req.UserName).ToListAsync();
        if (existing.Count > 0)
            return new List<string> { "Username taken" };

        existing = await _ctx.Members.Where(user => user.Email == req.Email).ToListAsync();
        if (existing.Count > 0)
            return new List<string> { "Email taken" };

        var salt = BCrypt.Net.BCrypt.GenerateSalt(13);
        var hash = BCrypt.Net.BCrypt.HashPassword(req.Password, salt);

        var member = new Member
        {
            Id = Guid.NewGuid(),
            UserName = req.UserName,
            FirstName = req.FirstName,
            LastName = req.LastName,
            Email = req.Email,
            Hash = hash,
            VendorId = req.VendorId
        };

        await _ctx.Members.AddAsync(member);
        await _ctx.SaveChangesAsync();

        return member;

    }

    public async Task<Either<List<string>, Member>> Get(UserLogin model)
    {
        var existing = _ctx.Members.Where(user => user.UserName == model.UserName);
        if (existing.Count() == 0)
            return new List<string> { "User by that username was not found" };

        var user = await existing.FirstAsync();

        if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Hash))
            return new List<string> { "Incorrect password" };

        return user;
    }

    public async Task<Either<List<string>, Member>> GetById(Guid id)
    {
        var existing = await _ctx.Members.Include(x => x.Vendor).Where(x => x.Id == id).ToListAsync();

        if (existing.Count == 0)
            return new List<string> { "User not found" };

        return existing.First();
    }
}