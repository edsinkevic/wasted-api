using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WastedApi.Database;
using WastedApi.Models;
using WastedApi.Requests;

namespace WastedApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OfferController : ControllerBase
{

    private readonly ILogger<OfferController> _logger;
    private readonly WastedContext _context;

    public OfferController(ILogger<OfferController> logger, WastedContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Offer>>> Get()
    {
        var offers = await _context.Offers.ToListAsync();

        return Ok(offers);
    }

    [HttpPost]
    public async Task<ActionResult<Offer>> Post([FromBody] OfferCreate request)
    {
        var existing = await _context.Offers.Where(offer => offer.Name == request.Name).ToListAsync();
        if (existing.Count > 0)
            return BadRequest();

        var offer = new Offer
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Weight = request.Weight,
            AddedBy = request.AddedBy,
            Category = request.Category
        };

        await _context.Offers.AddAsync(offer);
        await _context.SaveChangesAsync();

        return Ok(offer);
    }
}
