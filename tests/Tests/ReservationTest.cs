using System;
using System.Collections.Generic;
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


        var vendorCreate = new VendorCreate { Name = "Maxima" };
        var postVendor = (await _api.Vendors.Post(vendorCreate))
            .Result.As<OkObjectResult>().Value.As<Vendor>();

        var offerCreate = new OfferCreate
        {
            Name = "Food",
            AddedBy = postVendor.Id,
            Category = Category.Meat,
            Weight = 50,
            Price = 20.50F
        };

        var postOffer = (await _api.Offers.Post(offerCreate))
            .Result.As<OkObjectResult>().Value.As<Offer>();

        var entryCreate = new OfferEntryCreate
        {
            Expiry = DateOnly.FromDateTime(DateTime.Now.AddYears(2)),
            OfferId = postOffer.Id,
            Amount = 50
        };

        var postEntry1 = (await _api.OfferEntries.Post(entryCreate))
            .Result.As<OkObjectResult>().Value.As<OfferEntry>();

        var get1 = (await _api.OfferEntries.Get())
            .Result.As<OkObjectResult>().Value.As<List<OfferEntry>>();

        get1.Should().HaveCount(1);

        Assert.True(postEntry1.OfferId == postOffer.Id && postEntry1.Amount == entryCreate.Amount);

        var postEntry2 = (await _api.OfferEntries.Post(entryCreate))
            .Result.As<OkObjectResult>().Value.As<OfferEntry>();

        var get2 = (await _api.OfferEntries.Get())
            .Result.As<OkObjectResult>().Value.As<List<OfferEntry>>();

        get2.Should().HaveCount(1);

        Assert.True(postEntry2.OfferId == postOffer.Id && postEntry2.Amount == entryCreate.Amount * 2);

        var updateRequest = new OfferEntryUpdate
        {
            OfferEntryId = postEntry1.Id,
            Amount = 70,
            Expiry = postEntry1.Expiry
        };

        var put = (await _api.OfferEntries.Put(updateRequest))
            .Result.As<OkObjectResult>().Value.As<OfferEntry>();
        var get3 = (await _api.OfferEntries.Get())
            .Result.As<OkObjectResult>().Value.As<List<OfferEntry>>();

        Assert.True(get3[0].Amount == updateRequest.Amount && get3[0].Id == postEntry1.Id);

        var entryCreate2 = new OfferEntryCreate
        {
            Expiry = DateOnly.FromDateTime(DateTime.Now.AddDays(-3)),
            OfferId = postOffer.Id,
            Amount = 50
        };

        var postEntry3 = (await _api.OfferEntries.Post(entryCreate2))
            .Result.As<OkObjectResult>().Value.As<OfferEntry>();

        var get4 = (await _api.OfferEntries.Get())
            .Result.As<OkObjectResult>().Value.As<List<OfferEntry>>();

        get4.Should().HaveCount(2);

        var clean = (await _api.OfferEntries.Clean())
            .Result.As<OkObjectResult>().Value.As<CleanResult>();

        clean.CleanedAmount.Should().Be(1);

        var get5 = (await _api.OfferEntries.Get())
            .Result.As<OkObjectResult>().Value.As<List<OfferEntry>>();

        get5.Should().HaveCount(1);

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
            Items = new List<ReservationItemCreate> { new ReservationItemCreate { Amount = 5, Entry = get5[0] } }
        };

        var postRes = (await _api.Reservations.Post(reservation))
            .Result.As<OkObjectResult>().Value.As<Reservation>();

        Console.WriteLine(postRes);
    }


}