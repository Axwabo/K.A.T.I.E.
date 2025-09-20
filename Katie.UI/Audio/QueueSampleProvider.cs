using NAudio.Wave;

namespace Katie.UI.Audio;

public sealed partial class QueueSampleProvider : ObservableObject, ISampleProvider
{

    public IList<QueuedAnnouncement> List { get; }

    [ObservableProperty]
    private QueuedAnnouncement? _current;

    public WaveFormat WaveFormat { get; }

    public QueueSampleProvider(IList<QueuedAnnouncement> list, QueuedAnnouncement initial)
    {
        List = list;
        _current = initial;
        WaveFormat = initial.Provider.WaveFormat;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        if (Current == null)
            return 0;
        var read = Current.Provider.Read(buffer, offset, count);
        if (read >= count)
            return read;
        var index = List.IndexOf(Current);
        if (++index >= List.Count)
        {
            Current = null;
            return read;
        }

        Current = List[index];
        return read + Current.Provider.Read(buffer, offset + read, count - read);
    }

}
