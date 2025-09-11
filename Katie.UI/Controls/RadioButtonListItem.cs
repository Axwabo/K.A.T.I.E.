namespace Katie.UI.Controls;

public sealed class RadioButtonListItem : RadioButton
{

    private readonly RadioButtonList _list;
    private readonly object? _item;

    public RadioButtonListItem(RadioButtonList list, object? item)
    {
        _list = list;
        _item = item;
        Padding = default;
    }

    protected override Type StyleKeyOverride => typeof(RadioButton);

    protected override void Toggle()
    {
        base.Toggle();
        if (IsChecked.GetValueOrDefault())
            _list.SelectedValue = _item;
    }

}
