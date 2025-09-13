namespace Katie.Core.NumberParsing;

using Mapper = Func<char, string>;

public sealed record NumberSettings(
    Mapper Ten,
    Mapper TenExact,
    Mapper TenOrdinal,
    Mapper OneExact,
    Mapper OneOrdinal,
    string Hundred,
    bool OneBeforeHundred
);
