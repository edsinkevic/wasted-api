using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wasted.Interfaces;
using Wasted.Repositories;
using WastedApi.Database;
using WastedApi.Models;
using WastedApi.Requests;

namespace WastedApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OfferController : ControllerBase
{

    private readonly ILogger<OfferController> _logger;
    private readonly IOfferRepository _offers;

    public OfferController(ILogger<OfferController> logger, IOfferRepository offers)
    {
        _logger = logger;
        _offers = offers;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Offer>>> Get() =>
        Ok(await _offers.Get());

    [HttpGet("{name}")]
    public async Task<ActionResult<IEnumerable<Offer>>> GetByName(string name) =>
        Ok(await _offers.GetByVendorName(name));

    [HttpPost]
    public async Task<ActionResult<Offer>> Post([FromBody] OfferCreate request) =>
        (await _offers.Create(request))
            .Right<ActionResult<Offer>>(offer => Ok(offer))
            .Left(errors => Conflict(new { errors = errors }));

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] OfferUpdate request) =>
        (await _offers.Update(request))
            .Right<IActionResult>(offer => Ok(offer))
            .Left(errors => Conflict(new ErrorResponse { Errors = errors }));

    [HttpDelete("{id}")]
    public async Task<ActionResult<Offer>> Delete(string id) =>
        (await _offers.Delete(id))
            .Right<ActionResult<Offer>>(Offer => Ok(Offer))
            .Left(errors => Conflict(new ErrorResponse { Errors = errors }));
}
