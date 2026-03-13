using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Planner.Domain.Converters;

/// <summary>
/// Converte le date Jira (es. "2025-02-10T11:21:25.496+0100") in DateOnly?.
/// Jira usa un offset timezone senza ":" (es. +0100 invece di +01:00).
/// </summary>
public sealed class JiraDateOnlyConverter : JsonConverter<DateOnly?>
{
    private static readonly string[] Formats =
    [
        "yyyy-MM-dd'T'HH:mm:ss.fffzzz",
        "yyyy-MM-dd'T'HH:mm:ss.fffffffzzz",
        "yyyy-MM-dd'T'HH:mm:sszzz",
        "yyyy-MM-dd'T'HH:mm:ss.fffzz",
        "yyyy-MM-dd'T'HH:mm:sszz",
        "yyyy-MM-dd"
    ];

    public override DateOnly? Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var raw = reader.GetString();
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        // Normalizza l'offset: "+0100" → "+01:00"
        var normalized = NormalizeOffset(raw);

        if (DateTimeOffset.TryParseExact(normalized, Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dto))
            return DateOnly.FromDateTime(dto.DateTime);

        // Fallback generico
        if (DateTimeOffset.TryParse(normalized, CultureInfo.InvariantCulture, DateTimeStyles.None, out dto))
            return DateOnly.FromDateTime(dto.DateTime);

        return null;
    }

    public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd"));
    }

    /// <summary>
    /// Normalizza offset come "+0100" → "+01:00" e "-0530" → "-05:30".
    /// </summary>
    private static string NormalizeOffset(string value)
    {
        // Cerca un pattern tipo +HHMM o -HHMM alla fine della stringa
        if (value.Length < 5) return value;

        var last5 = value[^5..];
        if ((last5[0] == '+' || last5[0] == '-') &&
            char.IsDigit(last5[1]) && char.IsDigit(last5[2]) &&
            char.IsDigit(last5[3]) && char.IsDigit(last5[4]))
        {
            // Inserisci ":" → "+01:00"
            return string.Concat(value.AsSpan(0, value.Length - 2), ":", value.AsSpan(value.Length - 2));
        }

        return value;
    }
}

