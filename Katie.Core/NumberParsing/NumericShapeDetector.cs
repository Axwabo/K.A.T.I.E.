using System;
using Katie.Core.Extensions;

namespace Katie.Core.NumberParsing;

public static class NumericShapeDetector
{

    public static NumericTokenShape Identify(ReadOnlySpan<char> span, int length)
        => length < span.Length
            ? throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be less than the span length.")
            : length >= 5 && span[..2].IsDigit() && span[3] == ':' && span[2..5].IsDigit()
                ? NumericTokenShape.Time
                : !span[..length].IsDigit()
                    ? NumericTokenShape.None
                    : span.Length > length && span[length] == '.'
                        ? NumericTokenShape.Ordinal
                        : NumericTokenShape.Regular;

}
