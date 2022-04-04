
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WastedApi.Database;
using WastedApi.Models;
using WastedApi.Requests;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using WastedApi.Helpers;
using Wasted.Interfaces;

namespace WastedApi.Controllers;

[Route("[controller]")]
[ApiController]
public class MiscController : ControllerBase
{
    private readonly ILogger<MiscController> _logger;

    public MiscController(ILogger<MiscController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("cats")]
    public IActionResult GetCategories() =>
        Ok(System.Enum.GetNames(typeof(Category)));
}
