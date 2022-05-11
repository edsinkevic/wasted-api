using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Wasted.Tests.Api;
using WastedApi.Database;
using WastedApi.Models;
using WastedApi.Requests;
using Xunit;

namespace Tests;

[Collection("Database")]
public class ReservationSuite
{
    private readonly WastedContext _ctx;
    private readonly Api _api;

    public ReservationSuite()
    {
        _ctx = new WastedContext();
        _api = new Api(_ctx);
    }

    [Fact]
    public async void HappyPath()
    {
        _api.Clean();

        await EntrySuite.HappyPathReusable(_api);

        var getEntries = (await _api.OfferEntries.Get())
            .Result.As<OkObjectResult>().Value.As<List<OfferEntry>>();

        var entry = getEntries[0];

        var customerCreate = new CustomerSignup
        {
            UserName = "string",
            FirstName = "string",
            LastName = "string",
            Email = "string",
            Password = "string"
        };

        var postCust = (await _api.Auth.UserRegister(customerCreate))
            .Result.As<OkObjectResult>().Value.As<Customer>();

        var reservation = new ReservationCreate
        {
            CustomerId = postCust.Id,
            Items = new List<ReservationItemCreate> { new ReservationItemCreate { Amount = 65, EntryId = entry.Id, EntryAmount = entry.Amount } }
        };

        var postRes = (await _api.Reservations.Post(reservation))
            .Result.As<OkObjectResult>().Value.As<Reservation>();

        var getRes = (await _api.Reservations.Get(postCust.Id.ToString()))
            .Result.As<OkObjectResult>().Value.As<Reservation>();

        getRes.ReservationItems[0].Amount.Should().Be(65);
        getRes.ReservationItems[0].Entry.Amount.Should().Be(5);
    }


}