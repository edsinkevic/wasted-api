using LanguageExt;
using Wasted.Repositories;
using WastedApi.Models;
using WastedApi.Requests;

namespace WastedApi.Helpers;

public class SignupService
{
    private readonly MemberRepository _members;
    private readonly VendorRepository _vendors;

    public SignupService(MemberRepository members, VendorRepository vendors)
    {
        _members = members;
        _vendors = vendors;
    }

    public async Task<Either<List<string>, Member>> CreateMemberWithVendor(MemberSignupWithVendor signup) =>
    await _vendors.Create(new VendorCreate { Name = signup.VendorName })
        .BindT(vendor => _members.Create(signup.ToMemberSignup(vendor.Id), true));
}

