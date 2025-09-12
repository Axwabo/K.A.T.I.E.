using Avalonia.Controls.Primitives;

namespace Katie.UI.Controls;

public sealed class RadioButtonList : SelectingItemsControl
{

    public string? GroupName { get; set; }

    public Type? ItemStyleKey { get; set; }

    public Classes? ItemClasses { get; set; }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        var radio = new RadioButtonListItem(this, item, ItemStyleKey ?? typeof(RadioButton)) {GroupName = GroupName, IsChecked = item?.Equals(SelectedItem)};
        if (ItemClasses != null)
            radio.Classes.AddRange(ItemClasses);
        return radio;
    }

}
