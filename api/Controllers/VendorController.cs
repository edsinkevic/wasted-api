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
public class VendorController : ControllerBase
{

    private readonly ILogger<VendorController> _logger;
    private readonly IVendorRepository _vendors;

    public VendorController(ILogger<VendorController> logger, IVendorRepository vendors)
    {
        _logger = logger;
        _vendors = vendors;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vendor>>> Get()
    {
        return Ok(await _vendors.Get());
    }

    [HttpPost]
    public async Task<ActionResult<IEnumerable<Vendor>>> Post(VendorCreate request)
    {
        return Ok(await _vendors.Create(request));
    }

    [HttpGet]
    [Route("{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        return (await _vendors.GetByName(name))
            .Right<IActionResult>(offer => Ok(offer))
            .Left(errors => Conflict(new { errors = errors }));
    }
}
