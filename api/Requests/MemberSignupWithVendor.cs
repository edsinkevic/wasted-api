namespace WastedApi.Requests;
public class MemberSignupWithVendor : CustomerSignup
{
    public string VendorName { get; set; } = null!;

    public MemberSignup ToMemberSignup(Guid vendorId) => new MemberSignup
    {
        UserName = UserName,
        FirstName = FirstName,
        LastName = LastName,
        Email = Email,
        Password = Password,
        VendorId = vendorId
    };
}