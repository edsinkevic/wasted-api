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
    private readonly IReservationRepository _reservations;

    public ReservationController(ILogger<ReservationController> logger, IReservationRepository reservations)
    {
        _logger = logger;
        _reservations = reservations;
    }

    [HttpPost]
    public async Task<ActionResult<Reservation>> Post(ReservationCreate req) =>
        (await _reservations.Create(req))
            .Right<ActionResult<Reservation>>(res => Ok(res))
            .Left(errors => Conflict(new ErrorResponse { Errors = errors }));

    [HttpGet("{id}")]
    public async Task<ActionResult<Reservation>> Get(string id) =>
        (await _reservations.GetByCustomerId(id))
            .Right<ActionResult<Reservation>>(res => Ok(res))
            .Left(errors => Conflict(new ErrorResponse { Errors = errors }));


    [HttpPost("{code}")]
    public async Task<ActionResult<Reservation>> PostCode(string code) =>
        (await _reservations.CompleteReservation(code))
            .Right<ActionResult<Reservation>>(res => Ok(res))
            .Left(errors => Conflict(new ErrorResponse { Errors = errors }));

    [HttpDelete("{id}")]
    public async Task<ActionResult<Reservation>> Delete(string id) =>
        (await _reservations.CancelReservation(id))
            .Right<ActionResult<Reservation>>(res => Ok(res))
            .Left(errors => Conflict(new ErrorResponse { Errors = errors }));
}
