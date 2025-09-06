using Avalonia.Interactivity;
using Avalonia.Media;

namespace Katie.UI.Controls;

public sealed class LabelRadioButton : RadioButton
{

    protected override Type StyleKeyOverride => typeof(Label);

    protected override void OnIsCheckedChanged(RoutedEventArgs e)
    {
        base.OnIsCheckedChanged(e);
        BorderBrush = IsChecked.GetValueOrDefault() ? Brushes.GreenYellow : Brushes.Transparent;
    }

}
