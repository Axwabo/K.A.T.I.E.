using System.IO;
using Katie.NAudio.Phrases;

namespace Katie.UI.Browser;

public sealed class MemoryStreamPhrase : WaveStreamPhrase
{

    private readonly MemoryStream _source;

    public MemoryStreamPhrase(MemoryStream source, string name) : base(source, name) => _source = source;

    public ArraySegment<byte> Data => new(_source.GetBuffer(), 0, (int) _source.Length);

}
