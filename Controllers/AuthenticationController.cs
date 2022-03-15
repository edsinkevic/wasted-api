using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WastedApi.Database;
using WastedApi.Models;
using WastedApi.Requests;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
namespace WastedApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly WastedContext _context;
    private readonly IConfiguration _configuration;

    public AuthenticationController(WastedContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<User>> Login([FromBody] UserLogin model)
    {
        var existingUser = await _context.Users.Where(user => user.UserName == model.UserName).FirstAsync();
        if (existingUser == null)
            return BadRequest();

        if (!BCrypt.Net.BCrypt.Verify(model.Password, existingUser.Hash))
            return BadRequest();


        return Ok();
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
}