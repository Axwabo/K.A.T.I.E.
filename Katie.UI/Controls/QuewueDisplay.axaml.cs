using System.Threading;
using Avalonia.VisualTree;
using Katie.UI.Audio;

namespace Katie.UI.Controls;

public sealed partial class QuewueDisplay : UserControl
{

    private readonly CancellationTokenSource _cts = new();

    private QueuedAnnouncement? _announcement;

    private WeakReference<ProgressBar>? _progressBar;

    public QuewueDisplay()
    {
        InitializeComponent();
        UpdateAsync(_cts.Token).ConfigureAwait(false);
    }

    private async Task UpdateAsync(CancellationToken token)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));
        while (await timer.WaitForNextTickAsync(token))
            Dispatcher.UIThread.Post(Update);
    }

    private void Update()
    {
        var current = ((QueuePageViewModel) DataContext!).Current;
        if (_announcement == current)
        {
            UpdateCurrent();
            return;
        }

        _announcement = current;
        _progressBar = null;
        foreach (var visual in this.GetVisualDescendants())
        {
            if (visual is not Control control || !control.Classes.Contains("announcement"))
                continue;
            var isCurrent = ReferenceEquals(current, control.Tag);
            control.Classes.Set("current", isCurrent);
            if (!isCurrent || current is null)
                continue;
            control.Classes.Remove("queued");
            _progressBar = new WeakReference<ProgressBar>(control.FindDescendantOfType<ProgressBar>()!);
        }
    }

    private void UpdateCurrent()
    {
        if (_announcement is null || _progressBar is null || !_progressBar.TryGetTarget(out var progressBar))
            return;
        progressBar.Value = _announcement.CurrentTime / _announcement.TotalTime;
        progressBar.IsIndeterminate = _announcement.CurrentTime == TimeSpan.Zero;
    }

}
