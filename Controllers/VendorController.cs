using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WastedApi.Database;
using WastedApi.Models;
using WastedApi.Requests;

namespace WastedApi.Controllers;

[ApiController]
[Route("[controller]")]
public class VendorController : ControllerBase
{

    private readonly ILogger<VendorController> _logger;
    private readonly WastedContext _context;

    public VendorController(ILogger<VendorController> logger, WastedContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vendor>>> Get()
    {
        var vendors = await _context.Vendors.ToListAsync();

        return Ok(vendors);
    }

    [HttpPost]
    public async Task<ActionResult<IEnumerable<Vendor>>> Post(VendorCreate request)
    {
        var newVendor = new Vendor
        {
            Name = request.Name,
            Id = Guid.NewGuid()
        };

        await _context.Vendors.AddAsync(newVendor);
        await _context.SaveChangesAsync();

        return Ok(await _context.Vendors.ToListAsync());
    }


    [HttpGet]
    [Route("{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var existing = await _context.Vendors.Where(vendor => vendor.Name == name).ToListAsync();

        if (existing.Count > 0)
            return Ok(existing.First());

        return Conflict(new { errors = new List<string> { "Vendor not found" } });
    }
}
