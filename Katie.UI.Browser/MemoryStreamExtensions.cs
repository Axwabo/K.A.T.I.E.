using System.IO;

namespace Katie.UI.Browser;

public static class MemoryStreamExtensions
{

    public static ArraySegment<byte> ToArraySegment(this MemoryStream source) => new(source.GetBuffer(), 0, (int) source.Length);

}
