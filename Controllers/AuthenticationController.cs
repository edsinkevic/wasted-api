using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WastedApi.Database;
using WastedApi.Models;
using WastedApi.Requests;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using WastedApi.Helpers;

namespace WastedApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly WastedContext _context;
    private readonly JwtService _jwt;
    private readonly IConfiguration _configuration;

    public AuthenticationController(WastedContext context, JwtService jwt, IConfiguration configuration)
    {
        _context = context;
        _jwt = jwt;
        _configuration = configuration;
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<User>> Login([FromBody] UserLogin model)
    {
        var existing = _context.Users.Where(user => user.UserName == model.UserName);
        if (existing.Count() == 0)
            return BadRequest();

        var user = await existing.FirstAsync();

        if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Hash))
            return BadRequest();

        var jwt = _jwt.Generate(user.Id);

        Response.Cookies.Append("jwt", jwt, new CookieOptions
        {
            HttpOnly = true
        });

        return Ok(new
        {
            message = "success"
        });
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<User>> Register([FromBody] UserSignup model)
    {
        var existingUsers = await _context.Users.Where(user => user.UserName == model.UserName).ToListAsync();
        if (existingUsers.Count > 0)
            return StatusCode(StatusCodes.Status500InternalServerError, "Username taken");

        var salt = BCrypt.Net.BCrypt.GenerateSalt(13);
        var hash = BCrypt.Net.BCrypt.HashPassword(model.Password, salt);

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = model.UserName,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Role = model.Role,
            Hash = hash
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return Ok(user);
    }

    [HttpGet("user")]
    public IActionResult GetUser()
    {
        var jwt = Request.Cookies["jwt"];

        Console.WriteLine(jwt);

        if (jwt == null)
            return Unauthorized();

        try
        {
            var token = _jwt.Verify(jwt);
            var id = Guid.Parse(token.Issuer);
            var user = _context.Users.Find(id);

            return Ok(user);
        }
        catch (Exception)
        {
            return Unauthorized();
        }
    }
}