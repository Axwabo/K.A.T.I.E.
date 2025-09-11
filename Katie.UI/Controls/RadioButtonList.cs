using Avalonia.Controls.Primitives;

namespace Katie.UI.Controls;

public sealed class RadioButtonList : SelectingItemsControl
{

    public string? GroupName { get; set; }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        => new RadioButtonListItem(this, item) {GroupName = GroupName, IsChecked = item?.Equals(SelectedItem)};

}
