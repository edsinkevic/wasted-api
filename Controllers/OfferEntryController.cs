using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wasted.Interfaces;
using Wasted.Repositories;
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
    private readonly IOfferEntryRepository _entries;

    public OfferEntryController(ILogger<OfferEntryController> logger, IOfferEntryRepository entries)
    {
        _logger = logger;
        _entries = entries;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OfferEntry>>> Get() =>
        Ok(await _entries.Get());


    [HttpPost]
    public async Task<IActionResult> Post([FromBody] OfferEntryCreate request) =>
        (await _entries.Create(request))
            .Right<IActionResult>(entry => Ok(entry))
            .Left(errors => Conflict(new { errors = errors }));

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] OfferEntryUpdate request) =>
        (await _entries.Update(request))
            .Right<IActionResult>(entry => Ok(entry))
            .Left(errors => Conflict(new { errors = errors }));

    [HttpPost("clean")]
    public async Task<IActionResult> Clean()
    {
        await _entries.Clean();
        return Ok();
    }

}
