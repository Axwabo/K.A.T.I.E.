using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Metadata;

namespace Katie.UI.Controls;

public sealed class RadioButtonList : SelectingItemsControl
{

    public static readonly StyledProperty<IDataTemplate?> RadioButtonListItemTemplateProperty = AvaloniaProperty.Register<RadioButtonList, IDataTemplate?>(
        nameof(RadioButtonListItemTemplate),
        defaultBindingMode: BindingMode.TwoWay,
        coerce: CoerceSelectionBoxItemTemplate
    );

    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public IDataTemplate? RadioButtonListItemTemplate
    {
        get => GetValue(RadioButtonListItemTemplateProperty);
        set => SetValue(RadioButtonListItemTemplateProperty, value);
    }

    public string? GroupName { get; set; }

    private static IDataTemplate? CoerceSelectionBoxItemTemplate(AvaloniaObject obj, IDataTemplate? template)
        => template is null && obj is RadioButtonList list ? list.ItemTemplate : template;

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        => new RadioButtonListItem(this, item) {GroupName = GroupName, IsChecked = item?.Equals(SelectedItem)};

}
