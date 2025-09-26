using NAudio.Wave;

namespace Katie.UI.Audio;

public sealed partial class QueueSampleProvider : ISampleProvider
{

    public IList<QueuedAnnouncement> List { get; }

    public QueuedAnnouncement? Current { get; private set; }

    public WaveFormat WaveFormat { get; }

    public QueueSampleProvider(IList<QueuedAnnouncement> list, QueuedAnnouncement initial)
    {
        List = list;
        Current = initial;
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
