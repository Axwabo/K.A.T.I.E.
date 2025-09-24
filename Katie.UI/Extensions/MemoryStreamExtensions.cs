using System.IO;

namespace Katie.UI.Extensions;

public static class MemoryStreamExtensions
{

    public static ArraySegment<byte> ToArraySegment(this MemoryStream source) => new(source.GetBuffer(), 0, (int) source.Length);

}
