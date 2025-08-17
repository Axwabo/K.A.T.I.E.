using System;

namespace Katie.Core.Extensions;

public static class SpanExtensions
{

    private const string Delimiters = " ,.";

    public static int IndexOfWordDelimiter(this ReadOnlySpan<char> span, int start)
    {
        var indexInSlice = span[start..].IndexOfAny(Delimiters);
        return indexInSlice == -1 ? -1 : indexInSlice + start;
    }

    public static int LowercaseHashCode(this ReadOnlySpan<char> span)
    {
        var code = 0;
        foreach (var c in span)
            code = HashCode.Combine(code, char.ToLowerInvariant(c));
        return code;
    }

    public static ReadOnlySpan<char> TrimDelimeters(this ReadOnlySpan<char> span) => span.Trim(Delimiters);

}
