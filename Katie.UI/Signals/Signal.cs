using NAudio.Wave;

namespace Katie.UI.Signals;

public sealed record Signal(WaveFileReader Provider, string Name, TimeSpan Duration);
