namespace Katie.Core.Extensions;

public static class SpanExtensions
{

    public const string Delimiters = " ,.";

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

    public static int LowercaseHashCode(this ReadOnlySpan<char> first, ReadOnlySpan<char> second)
    {
        var code = first.LowercaseHashCode();
        foreach (var c in second)
            code = HashCode.Combine(code, char.ToLowerInvariant(c));
        return code;
    }

    public static ReadOnlySpan<char> TrimDelimeters(this ReadOnlySpan<char> span) => span.Trim(Delimiters);

    public static bool IsDigit(this ReadOnlySpan<char> span)
    {
        foreach (var c in span)
            if (!char.IsDigit(c))
                return false;
        return true;
    }

}
