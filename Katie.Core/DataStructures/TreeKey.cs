using Katie.Core.Extensions;

namespace Katie.Core.DataStructures;

// ReSharper disable once StructLacksIEquatable.Global
public readonly ref struct TreeKey
{

    public ReadOnlySpan<char> First { get; init; }

    public ReadOnlySpan<char> Second { get; init; }

    public int Length => First.Length + Second.Length;

    public override int GetHashCode() => First.LowercaseHashCode(Second);

    public override string ToString() => Second.IsEmpty ? First.ToString() : First.ToString() + Second.ToString();

    public static implicit operator TreeKey(ReadOnlySpan<char> span) => new() {First = span};

    public static implicit operator TreeKey(string s) => s.AsSpan();

}
