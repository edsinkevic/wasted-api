using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WastedApi.Database;
using WastedApi.Extensions;
using WastedApi.Models;
using WastedApi.Requests;

namespace WastedApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OfferEntryController : ControllerBase
{

    private readonly ILogger<OfferEntryController> _logger;
    private readonly WastedContext _context;

    public OfferEntryController(ILogger<OfferEntryController> logger, WastedContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OfferEntry>>> Get()
    {
        var offerEntries = await _context.OfferEntries.Include(item => item.Offer).ThenInclude(offer => offer.Vendor).ToListAsync();

        return Ok(offerEntries);
    }


    [HttpPost]
    public async Task<ActionResult<IEnumerable<OfferEntry>>> Post([FromBody] OfferEntryCreate request)
    {
        var existing = _context.OfferEntries.Where(item => item.OfferId == request.OfferId && item.Expiry == request.Expiry);
        if (existing.Count() > 0)
        {
            var updated = await existing.FirstAsync();

            updated.Amount = updated.Amount + request.Amount;

            await _context.SaveChangesAsync();

            return Ok(updated);
        }

        var item = new OfferEntry
        {
            Amount = request.Amount,
            OfferId = request.OfferId,
            Added = DateTime.Now.ToUnspecified(),
            Expiry = request.Expiry.ToUnspecified(),
            Id = Guid.NewGuid()
        };

        await _context.OfferEntries.AddAsync(item);
        await _context.SaveChangesAsync();

        return Ok(item);
    }


}
