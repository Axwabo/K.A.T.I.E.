using System.ComponentModel;

namespace Katie.UI.Controls;

public sealed partial class QuewueDisplay : UserControl
{

    public QuewueDisplay() => InitializeComponent();

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is not QueuePageViewModel vm)
            return;
        vm.PropertyChanged += VmOnPropertyChanged;
    }

    private void VmOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(QueuePageViewModel.Current))
            SetHighlight(((QueuePageViewModel) sender!).Current);
    }

    private void SetHighlight(object? announcement)
    {
        var children = Items.Presenter.Panel.Children;
    }

}
