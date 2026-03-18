using System.Text.RegularExpressions;
using Planner.Components.Shared.Filters.Model;

namespace Planner.Extensions;

public static partial class EnumExtensions
{
    public static string ToTitleCase(this DatePreset.Preset preset)
    {
        if (preset == DatePreset.Preset.None) return "Any Time";

        var name = $"{preset}";
        name = MyRegex().Replace(name, "$1 $2");
        name = MyRegex1().Replace(name, "$1 $2");
        
        return name;
    }

    [GeneratedRegex("([a-z])([A-Z0-9])")]
    private static partial Regex MyRegex();
    [GeneratedRegex("([0-9])([A-Z])")]
    private static partial Regex MyRegex1();
}