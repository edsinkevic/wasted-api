using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WastedApi.Database;

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
        var offerEntries = await _context.OfferEntries.ToListAsync();

        return Ok(offerEntries);
    }

}
