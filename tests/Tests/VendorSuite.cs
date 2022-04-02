using System;
using LanguageExt;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Wasted.Interfaces;
using Wasted.Repositories;
using Wasted.Tests.Mocks;
using WastedApi.Controllers;
using WastedApi.Models;
using WastedApi.Requests;
using Xunit;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Wasted.Tests.Api;

namespace Tests;

[Collection("Database")]
public class VendorSuite
{
    private readonly WastedContext _ctx;
    private readonly Api _api;

    public VendorSuite()
    {
        _ctx = new WastedContext();
        _api = new Api(_ctx);
    }

    [Fact]
    public async void HappyPath()
    {
        _api.Clean();

        var create1 = new VendorCreate
        {
            Name = "Maxima"
        };

        var create2 = new VendorCreate
        {
            Name = "Iki"
        };

        var post1 = (await _api.Vendors.Post(create1))
            .Result.As<OkObjectResult>();

        post1.StatusCode.Should().Be(200);

        var get1 = (await _api.Vendors.Get())
            .Result.As<OkObjectResult>().Value.As<List<Vendor>>();

        get1.Should().AllSatisfy(vendor => vendor.Name.Should().Be("Maxima"));
        get1.Should().HaveCount(1);

        var post2 = (await _api.Vendors.Post(create2))
            .Result.As<OkObjectResult>();

        post2.StatusCode.Should().Be(200);

        var get2 = (await _api.Vendors.Get())
            .Result.As<OkObjectResult>().Value.As<List<Vendor>>();

        get2.Should().HaveCount(2);

        var getByName = (await _api.Vendors.GetByName("Iki"))
            .Result.As<OkObjectResult>().Value.As<Vendor>();

        Assert.True(getByName.Name == create2.Name);

        _api.Clean();
    }

    [Fact]
    public async void IncorrectModel()
    {
        var invalidModel = new VendorCreate
        {
            Name = ""
        };

        var post = (await _api.Vendors.Post(invalidModel))
            .Result.As<ConflictObjectResult>().Value.As<ErrorResponse>();

        Assert.True(post.errors[0] == "Vendor name cannot be empty!");

        _api.Clean();
    }

    [Fact]
    public async void VendorDoesntExist()
    {
        _api.Clean();

        var get = (await _api.Vendors.GetByName("Maxima"))
            .Result.As<ConflictObjectResult>().Value.As<ErrorResponse>();

        Assert.True(get.errors[0] == "Vendor not found");

    }

}