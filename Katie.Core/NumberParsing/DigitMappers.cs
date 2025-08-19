using System;

namespace Katie.Core.NumberParsing;

using Mapper = Func<char, string>;

public sealed record DigitMappers(
    Mapper Ten,
    Mapper TenExact,
    Mapper TenOrdinal,
    Mapper OneExact,
    Mapper OneOrdinal
);
