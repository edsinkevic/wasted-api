using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Wasted.Tests.Api;
using Wasted.Tests.Mocks;
using WastedApi.Models;
using WastedApi.Requests;
using Xunit;

namespace Tests;

[Collection("Database")]
public class EntrySuite
{
    private readonly WastedContext _ctx;
    private readonly Api _api;

    public EntrySuite()
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
            Expiry = DateTime.Now.AddYears(2),
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
    }


}