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

public class ApiTests
{
    private readonly WastedContext _ctx;
    private readonly Api _api;

    public ApiTests()
    {
        _ctx = new WastedContext();
        _api = new Api(_ctx);
    }

    [Fact]
    public async void VendorTest()
    {
        var l = new Mock<ILogger<VendorController>>();
        _ctx.Database.ExecuteSqlRaw("TRUNCATE TABLE VENDORS CASCADE;");
        var v = new VendorRepository(_ctx);
        var api = new VendorController(l.Object, v);

        var create = new VendorCreate
        {
            Name = "Maxima"
        };

        var post = (await api.Post(create))
            .Result
            .As<OkObjectResult>();

        post.StatusCode.Should().Be(200);


        var get = (await api.Get())
            .Result
            .As<OkObjectResult>()
            .Value
            .As<List<Vendor>>();

        get.Should().AllSatisfy(vendor => vendor.Name.Should().Be("Maxima"));
        get.Should().HaveCount(1);
    }
}