namespace Katie.Core.Extensions;

public static class SpanExtensions
{

    public const string Delimiters = " ,.?!";

    extension(ReadOnlySpan<char> span)
    {

        public int IndexOfWordDelimiter(int start)
        {
            var indexInSlice = span[start..].IndexOfAny(Delimiters);
            return indexInSlice == -1 ? -1 : indexInSlice + start;
        }

        public int LowercaseHashCode()
        {
            var code = 0;
            foreach (var c in span)
                code = HashCode.Combine(code, char.ToLowerInvariant(c));
            return code;
        }

        public int LowercaseHashCode(ReadOnlySpan<char> second)
        {
            var code = span.LowercaseHashCode();
            foreach (var c in second)
                code = HashCode.Combine(code, char.ToLowerInvariant(c));
            return code;
        }

        public ReadOnlySpan<char> TrimDelimeters() => span.Trim(Delimiters);

        public bool IsDigit()
        {
            foreach (var c in span)
                if (!char.IsDigit(c))
                    return false;
            return true;
        }

        public int Count(char c, Index start)
        {
            var count = 0;
            for (var i = start.GetOffset(span.Length); i < span.Length; i++)
                if (span[i] == c)
                    count++;
            return count;
        }

    }

}
