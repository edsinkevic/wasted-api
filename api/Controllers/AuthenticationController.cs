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
public class AuthenticationController : ControllerBase
{
    private string secureMemberKey = "very secure member lmao";
    private string secureUserKey = "very secure user lmao";
    private readonly ILogger<AuthenticationController> _logger;
    private readonly JwtService _jwt;
    private readonly IMemberRepository _members;
    private readonly ICustomerRepository _customers;
    private readonly SignupService _signups;

    public AuthenticationController(ILogger<AuthenticationController> logger, JwtService jwt, IMemberRepository members, ICustomerRepository customers, SignupService signups)
    {
        _logger = logger;
        _jwt = jwt;
        _members = members;
        _customers = customers;
        _signups = signups;
    }

    [HttpPost]
    [Route("member/register")]
    public async Task<IActionResult> MemberRegister([FromBody] MemberSignup model) =>
        (await _members.Create(model, false))
            .Right<IActionResult>(member => Ok(member))
            .Left(errors => Conflict(new { errors = errors }));

    [HttpPost]
    [Route("member/register/vendor")]
    public async Task<ActionResult<Member>> MemberRegisterWithVendor([FromBody] MemberSignupWithVendor model) =>
    (await _signups.CreateMemberWithVendor(model))
        .Right<ActionResult>(member => Ok(member))
        .Left(errors => Conflict(new { errors = errors }));

    [HttpPost]
    [Route("member/login")]
    public async Task<IActionResult> MemberLogin([FromBody] UserLogin model) =>
        (await _members.Get(model)).Right<IActionResult>(member =>
            {
                var jwt = _jwt.Generate(member.Id, secureMemberKey);

                Response.Cookies.Append("jwt", jwt, new CookieOptions
                {
                    HttpOnly = true
                });

                return Ok(new
                {
                    message = "success"
                });

            })
                .Left(errors => Unauthorized(new { errors = errors }));



    [HttpGet("member")]
    public async Task<IActionResult> GetMember()
    {
        var jwt = Request.Cookies["jwt"];

        Console.WriteLine(jwt);

        if (jwt == null)
            return Unauthorized();

        try
        {
            var token = _jwt.Verify(jwt, secureMemberKey);
            var id = Guid.Parse(token.Issuer);

            var result = await _members.GetById(id);
            return result
                .Right<IActionResult>(member => Ok(member))
                .Left(errors => Unauthorized(new { errors = errors }));
        }
        catch (Exception)
        {
            return Unauthorized();
        }
    }


    [HttpPost]
    [Route("user/login")]
    public async Task<IActionResult> UserLogin([FromBody] UserLogin model)
    {
        var result = await _customers.Get(model);

        return result.Right<IActionResult>(member =>
        {
            var jwt = _jwt.Generate(member.Id, secureUserKey);

            Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = true
            });

            return Ok(new
            {
                message = "success"
            });

        })
            .Left(errors => Unauthorized(new { errors = errors }));
    }

    [HttpPost]
    [Route("user/register")]
    public async Task<ActionResult<Customer>> UserRegister([FromBody] CustomerSignup model) =>
        (await _customers.Create(model))
            .Right<ActionResult<Customer>>(member => Ok(member))
            .Left(errors => Conflict(new ErrorResponse { Errors = errors }));

    [HttpGet("user")]
    public async Task<IActionResult> GetUser()
    {
        var jwt = Request.Cookies["jwt"];

        Console.WriteLine(jwt);

        if (jwt == null)
            return Unauthorized();

        try
        {
            var token = _jwt.Verify(jwt, secureUserKey);
            var id = Guid.Parse(token.Issuer);
            var result = await _customers.GetById(id);
            return result
                .Right<IActionResult>(customer => Ok(customer))
                .Left(errors => Unauthorized(new { errors = errors }));
        }
        catch (Exception)
        {
            return Unauthorized();
        }
    }
}
