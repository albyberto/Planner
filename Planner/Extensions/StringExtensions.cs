using System.Globalization;

namespace Planner.Extensions;

public static class StringExtensions
{
    public static string ToTitleCase(this string input) => string.IsNullOrWhiteSpace(input) 
        ? input 
        : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
}