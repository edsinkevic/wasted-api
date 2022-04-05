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
public class ReservationController : ControllerBase
{

    private readonly ILogger<ReservationController> _logger;

    public ReservationController(ILogger<ReservationController> logger)
    {
        _logger = logger;
    }
}
