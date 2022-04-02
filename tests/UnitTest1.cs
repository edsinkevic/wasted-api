using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Wasted.Interfaces;
using WastedApi.Controllers;
using WastedApi.Models;
using WastedApi.Requests;
using Xunit;

namespace Tests;

public class UnitTest1
{
    [Fact]
    public async void Test1()
    {
        var create = new VendorCreate
        {
            Name = "Maxima"
        };
        var r = new Mock<IVendorRepository>();
        r.Setup(service => service.Create(create)).ReturnsAsync(new Vendor { Name = "Maxima" });
        var l = new Mock<ILogger<VendorController>>();
        var c = new VendorController(l.Object, r.Object);

        var result = (OkObjectResult)await c.Post(create);
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<Vendor>();
    }
}