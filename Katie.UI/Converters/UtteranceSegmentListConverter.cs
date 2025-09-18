using System.Globalization;
using Avalonia.Data.Converters;
using Katie.Core;

namespace Katie.UI.Converters;

public sealed class UtteranceSegmentListConverter<T> : IValueConverter where T : PhraseBase
{

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is IEnumerable<UtteranceSegment<T>> enumerable
            ? string.Join("\n", enumerable.Select(e => e.Phrase?.Text ?? $"[{e.Duration.TotalSeconds}s pause]"))
            : null;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();

}
