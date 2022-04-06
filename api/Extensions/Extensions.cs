namespace WastedApi.Extensions;

public static class Extensions
{

    // public static DateTime ToUnspecified(this DateTime date)
    // {
    //     return DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
    // }
    public static DateTime ToUnspecified(this DateTime date)
    {
        var NewDT = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond, DateTimeKind.Unspecified);
        return NewDT;
    }

    public static DateTime ToUtc(this DateTime date)
    {
        var NewDT = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond, DateTimeKind.Utc);
        return NewDT;
    }

}