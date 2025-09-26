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
        foreach (var visual in this.GetVisualDescendants())
        {
            if (visual is not Control control || !control.Classes.Contains("announcement"))
                continue;
            var isCurrent = ReferenceEquals(current, control.Tag);
            control.Classes.Set("current", isCurrent);
            if (!isCurrent || current is null)
                continue;
            control.Classes.Remove("queued");
            var progressBar = control.FindDescendantOfType<ProgressBar>()!;
            progressBar.Value = current.CurrentTime / current.TotalTime;
            progressBar.IsIndeterminate = current.CurrentTime == TimeSpan.Zero;
        }
    }

}
