using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace WastedApi.Converters;
public class DateOnlyConverter : JsonConverter<DateOnly>
{
    private readonly JsonSerializerOptions ConverterOptions;
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        return DateOnly.FromDateTime(DateTime.Parse(reader.GetString()!));
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        JsonSerializer.Serialize<DateTime>(writer, value.ToDateTime(TimeOnly.MinValue), ConverterOptions);
    }
}