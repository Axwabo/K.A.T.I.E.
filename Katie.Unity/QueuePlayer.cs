using UnityEngine;

namespace Katie.Unity;

[RequireComponent(typeof(AudioSource))]
public sealed class QueuePlayer : MonoBehaviour
{

    private AudioSource _source = null!;

    private double _playAt;

    private readonly Queue<(AudioClip?, float)> _queue = new();

    public double EndDspTime
    {
        get
        {
            var end = _playAt;
            foreach (var (_, duration) in _queue) 
                end += duration;
            return end;
        }
    }

    public bool IsYapping => _playAt > AudioSettings.dspTime || _source.isPlaying;

    private void Awake() => _source = GetComponent<AudioSource>();

    public void Enqueue(AudioClip? clip)
    {
        if (clip)
            Enqueue(clip, clip.length);
    }

    public void Enqueue(AudioClip? clip, float length)
    {
        _queue.Enqueue((clip, length));
        if (!IsYapping)
            _playAt = AudioSettings.dspTime + length;
    }

    public void Delay(float length) => Enqueue(null, length);

    private void Update()
    {
        var dspTime = AudioSettings.dspTime;
        if (_playAt > dspTime || !_queue.TryDequeue(out var tuple))
            return;
        if (tuple.Item1)
            _source.PlayOneShot(tuple.Item1);
        _playAt = dspTime + tuple.Item2;
    }

    public void Clear()
    {
        _playAt = 0;
        _queue.Clear();
        _source.Stop();
    }

}
