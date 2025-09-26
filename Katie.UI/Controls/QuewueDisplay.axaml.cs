using System.Threading;
using Avalonia.VisualTree;

namespace Katie.UI.Controls;

public sealed partial class QuewueDisplay : UserControl
{

    private readonly CancellationTokenSource _cts = new();

    public QuewueDisplay()
    {
        InitializeComponent();
        UpdateAsync(_cts.Token).ConfigureAwait(false);
    }

    private async Task UpdateAsync(CancellationToken token)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));
        while (await timer.WaitForNextTickAsync(token))
            Dispatcher.UIThread.Post(SetHighlight);
    }

    private void SetHighlight()
    {
        var current = ((QueuePageViewModel) DataContext!).Current;
        foreach (var control in this.GetVisualDescendants())
            if (control is Grid grid)
                grid.Classes.Set("current", ReferenceEquals(current, grid.Tag));
    }

}
