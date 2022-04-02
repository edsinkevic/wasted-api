
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Wasted.Database.Interfaces;
using Wasted.Interfaces;
using WastedApi.Database;
using WastedApi.Models;
using WastedApi.Requests;

namespace Wasted.Repositories;

public class VendorRepository : IVendorRepository
{
    private readonly IWastedContext _ctx;

    public VendorRepository(IWastedContext ctx)
    {
        _ctx = ctx;
    }
    public async Task<Either<List<string>, Vendor>> Create(VendorCreate req)
    {
        var errors = req.isValid();
        if (errors.Count > 0)
            return errors;

        var newVendor = new Vendor
        {
            Name = req.Name,
            Id = Guid.NewGuid()
        };

        await _ctx.Vendors.AddAsync(newVendor);
        await _ctx.SaveChangesAsync();

        return newVendor;
    }

    public async Task<IEnumerable<Vendor>> Get() =>
        await _ctx.Vendors.ToListAsync();

    public async Task<Either<List<string>, Vendor>> GetByName(string name)
    {
        var existing = await _ctx.Vendors.Where(vendor => vendor.Name == name).ToListAsync();
        if (existing.Count > 0)
            return existing.First();

        return new List<string> { "Vendor not found" };
    }
}