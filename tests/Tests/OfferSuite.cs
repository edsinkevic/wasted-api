using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Wasted.Tests.Api;
using WastedApi.Database;
using WastedApi.Models;
using WastedApi.Requests;
using Xunit;

namespace Tests;

[Collection("Database")]
public class OfferSuite
{
    private readonly WastedContext _ctx;
    private readonly Api _api;

    public OfferSuite()
    {
        _ctx = new WastedContext();
        _api = new Api(_ctx);
    }

    [Fact]
    public async void AddOfferHappyPath()
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

        Assert.True(
            postOffer.Name == offerCreate.Name &&
             offerCreate.AddedBy == postVendor.Id &&
             postOffer.Price == offerCreate.Price &&
             postOffer.Category == offerCreate.Category
             );

    }

}