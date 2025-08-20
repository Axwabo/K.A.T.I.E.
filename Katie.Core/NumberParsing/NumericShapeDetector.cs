using Katie.Core.Extensions;

namespace Katie.Core.NumberParsing;

public static class NumericShapeDetector
{

    public static NumericTokenShape Identify(ReadOnlySpan<char> span, int length)
        => length > span.Length
            ? throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be greater than the span length.")
            : length >= 5 && span[..2].IsDigit() && span[2] == ':' && span[3..5].IsDigit()
                ? IdentifyTime(span[3..5])
                : !span[..length].IsDigit()
                    ? IdentifyNotFullyDigit(span[..length])
                    : span.Length > length && span[length] == '.'
                        ? NumericTokenShape.Ordinal
                        : NumericTokenShape.Regular;

    private static NumericTokenShape IdentifyTime(ReadOnlySpan<char> minute)
        => minute[0] == '0' && minute[1] == '0'
            ? NumericTokenShape.TimeHourOnly
            : NumericTokenShape.TimeHourMinute;

    private static NumericTokenShape IdentifyNotFullyDigit(ReadOnlySpan<char> span)
        => span.IndexOf('-') > 0
            ? NumericTokenShape.RegularSuffixed
            : NumericTokenShape.None;

}
