namespace WastedApi.Extensions;

public static class Extensions
{

    public static DateTime ToUnspecified(this DateTime date)
    {
        return DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
    }

}