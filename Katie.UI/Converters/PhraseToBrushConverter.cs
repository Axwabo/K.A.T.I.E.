using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Katie.UI.Converters;

public sealed class PhraseToBrushConverter : IValueConverter
{

    private static readonly ImmutableSolidColorBrush Cached = new(Color.FromArgb(20, 0, 255, 0));
    private static readonly ImmutableSolidColorBrush NotCached = new(Color.FromArgb(20, 255, 0, 0));

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => !targetType.IsAssignableFrom(typeof(IBrush))
            ? throw new ArgumentException("Target must be an IBrush", nameof(targetType))
            : value is RawSourceSamplePhrase
                ? Cached
                : NotCached;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();

}
