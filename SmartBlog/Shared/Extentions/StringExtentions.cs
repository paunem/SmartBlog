using System.Text.Json;

namespace SmartBlog.Shared.Extentions;

public static class StringExtentions
{
    public static T FromJson<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }

    public static string ToJson<T>(this T value)
    {
        return JsonSerializer.Serialize(value);
    }
}
