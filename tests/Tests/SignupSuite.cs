using System;
using System.Collections.Generic;
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
public class SignupSuite
{
    private readonly WastedContext _ctx;
    private readonly Api _api;

    public SignupSuite()
    {
        _ctx = new WastedContext();
        _api = new Api(_ctx);
    }

    [Fact]
    public async void Test()
    {
        var signup = new MemberSignupWithVendor
        {
            UserName = "beanjean",
            FirstName = "Bean",
            LastName = "Jean",
            Email = "jeanbean@gmail.com",
            Password = "12345",
            VendorName = "JeanBeanFarm"
        };

        (await _api.Auth.MemberRegisterWithVendor(signup))
            .Result.As<ObjectResult>().Value.As<Member>().Should().NotBeNull();
    }
}