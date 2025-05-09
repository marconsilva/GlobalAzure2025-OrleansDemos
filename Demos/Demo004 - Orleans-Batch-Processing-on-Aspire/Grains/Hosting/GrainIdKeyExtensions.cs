using System.Text;

namespace Orleans.Runtime;

public static class GrainIdKeyExtensions
{
    public static string? GetKeyExtension(this GrainId grainId) => GetExtension(grainId.Key);

    public static string? GetExtension(this IdSpan key)
    {
        var keyString = key.AsSpan();
        return keyString.IndexOf((byte)'+') is int index && index >= 0
            ? Encoding.UTF8.GetString(keyString[(index + 1)..]) : default;
    }
}
