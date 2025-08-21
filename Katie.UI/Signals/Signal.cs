namespace Katie.UI.Signals;

public sealed record Signal(RawSourceSampleProvider Provider, string Name, TimeSpan Duration);
