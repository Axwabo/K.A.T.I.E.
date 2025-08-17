using System;

namespace Katie.Core;

public interface IPhrase
{

    string Text { get; }

    TimeSpan Duration { get; }

}
