using System.Globalization;

namespace Planner.Extensions;

public static class StringExtensions
{
    extension(string input)
    {
        public string ToTitleCase() => string.IsNullOrWhiteSpace(input) 
            ? input 
            : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
    }
}