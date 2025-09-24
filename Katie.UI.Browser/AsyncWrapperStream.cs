using System.IO;

namespace Katie.UI.Browser;

public sealed class AsyncWrapperStream : Stream
{

    private readonly Stream _source;

    public AsyncWrapperStream(Stream source) => _source = source;

    public override void Flush() => _source.FlushAsync().Wait();

    public override int Read(byte[] buffer, int offset, int count) => _source.ReadAsync(buffer, offset, count).Result;

    public override long Seek(long offset, SeekOrigin origin) => _source.Seek(offset, origin);

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override bool CanRead => _source.CanRead;
    public override bool CanSeek => _source.CanSeek;
    public override bool CanWrite => false;
    public override long Length => _source.Length;

    public override long Position
    {
        get => _source.Position;
        set => _source.Position = value;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            base.Dispose();
    }

}
