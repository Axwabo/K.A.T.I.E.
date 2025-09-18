using System.Globalization;
using Avalonia.Data.Converters;

namespace Katie.UI.Converters;

public sealed class PhraseToTextConverter : IValueConverter
{

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => (value as WavePhraseBase)?.Text ?? parameter;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();

}
