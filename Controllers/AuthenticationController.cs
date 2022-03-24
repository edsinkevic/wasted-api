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
    private string secureMemberKey = "very secure member lmao";
    private string secureUserKey = "very secure user lmao";
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
    [Route("member/register")]
    public async Task<ActionResult<Customer>> MemberRegister([FromBody] MemberSignup model)
    {
        var errors = model.isValid();

        if (errors.Count > 0)
            return Conflict(new { errors = errors });

        var existing = await _context.Members.Where(user => user.UserName == model.UserName).ToListAsync();
        if (existing.Count > 0)
            return Conflict(new { errors = new List<string> { "Username taken" } });

        existing = await _context.Members.Where(user => user.Email == model.Email).ToListAsync();
        if (existing.Count > 0)
            return Conflict(new { errors = new List<string> { "Email taken" } });

        var salt = BCrypt.Net.BCrypt.GenerateSalt(13);
        var hash = BCrypt.Net.BCrypt.HashPassword(model.Password, salt);

        var user = new Member
        {
            Id = Guid.NewGuid(),
            UserName = model.UserName,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Hash = hash,
            VendorId = model.VendorId
        };

        await _context.Members.AddAsync(user);
        await _context.SaveChangesAsync();

        return Ok(user);
    }


    [HttpPost]
    [Route("member/login")]
    public async Task<IActionResult> MemberLogin([FromBody] UserLogin model)
    {
        var existing = _context.Members.Where(member => member.UserName == model.UserName);
        if (existing.Count() == 0)
            return Unauthorized(new { errors = new List<string> { "User by that username was not found" } });

        var user = await existing.FirstAsync();

        if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Hash))
            return Unauthorized(new { errors = new List<string> { "Incorrect password" } });

        var jwt = _jwt.Generate(user.Id, secureMemberKey);

        Response.Cookies.Append("jwt", jwt, new CookieOptions
        {
            HttpOnly = true
        });

        return Ok(new
        {
            message = "success"
        });
    }


    [HttpGet("member")]
    public IActionResult GetMember()
    {
        var jwt = Request.Cookies["jwt"];

        Console.WriteLine(jwt);

        if (jwt == null)
            return Unauthorized();

        try
        {
            var token = _jwt.Verify(jwt, secureMemberKey);
            var id = Guid.Parse(token.Issuer);
            var existing = _context.Members.Include(member => member.Vendor).Where(member => member.Id == id).ToList();

            if (existing.Count == 0)
                return Unauthorized();

            return Ok(existing[0]);
        }
        catch (Exception)
        {
            return Unauthorized();
        }
    }


    [HttpPost]
    [Route("user/login")]
    public async Task<ActionResult<Customer>> UserLogin([FromBody] UserLogin model)
    {
        var existing = _context.Users.Where(user => user.UserName == model.UserName);
        if (existing.Count() == 0)
            return Unauthorized(new { errors = new List<string> { "User by that username was not found" } });

        var user = await existing.FirstAsync();

        if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Hash))
            return Unauthorized(new { errors = new List<string> { "Incorrect password" } });

        var jwt = _jwt.Generate(user.Id, secureUserKey);

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
    [Route("user/register")]
    public async Task<ActionResult<Customer>> UserRegister([FromBody] CustomerSignup model)
    {
        var errors = model.isValid();

        if (errors.Count > 0)
            return Conflict(new { errors = errors });

        var existingUsers = await _context.Users.Where(user => user.UserName == model.UserName).ToListAsync();
        if (existingUsers.Count > 0)
            return Conflict(new { errors = new List<string> { "Username taken" } });

        existingUsers = await _context.Users.Where(user => user.Email == model.Email).ToListAsync();
        if (existingUsers.Count > 0)
            return Conflict(new { errors = new List<string> { "Email taken" } });

        var salt = BCrypt.Net.BCrypt.GenerateSalt(13);
        var hash = BCrypt.Net.BCrypt.HashPassword(model.Password, salt);

        var user = new Customer
        {
            Id = Guid.NewGuid(),
            UserName = model.UserName,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
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
            var token = _jwt.Verify(jwt, secureUserKey);
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
