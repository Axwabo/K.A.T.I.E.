using Katie.Core.DataStructures;
using Katie.Core.Extensions;

namespace Katie.Core.NumberParsing;

public ref struct SequentialNumberParser<T> where T : PhraseBase
{

    public const int MaxDigits = 4;

    private readonly ReadOnlySpan<char> _text;
    private readonly PhraseTree<T> _tree;
    private readonly NumberSettings _settings;

    public NumberInterpretation Interpretation { get; }

    public bool Thousand { get; private set; }

    public bool Hundred { get; private set; }

    public int PositionalIndex { get; private set; }

    public char Digit => IsActive
        ? _text[Math.Max(0, _text.Length - PositionalIndex - 1)]
        : throw new InvalidOperationException("Cannot access the digit on an inactive parser");

    public bool IsActive => (Thousand || Hundred || PositionalIndex != -1) && !_text.IsEmpty;

    public SequentialNumberParser(ReadOnlySpan<char> text, PhraseTree<T> tree, NumberSettings settings, NumberInterpretation interpretation)
    {
        if (text.IsEmpty)
            throw new ArgumentException("Number text cannot be empty", nameof(text));
        if (interpretation != NumberInterpretation.SeparateDigits && text.Length > MaxDigits)
            throw new NotSupportedException($"Cannot parse a number of {text.Length} digits");
        if (!text.IsDigit())
            throw new ArgumentException("Number text must contain only digits", nameof(text));
        _text = text;
        _tree = tree;
        _settings = settings;
        Interpretation = interpretation;
        PositionalIndex = _text.Length - 1;
    }

    public bool Next(out UtteranceSegment<T> phrase, out int advanced)
    {
        if (Interpretation == NumberInterpretation.SeparateDigits)
            return NextDigit(out phrase, out advanced);
        if (Thousand)
        {
            phrase = _tree.RootPhrase(_settings.Thousand);
            advanced = 0;
            Thousand = false;
            return true;
        }

        if (Hundred)
        {
            phrase = _tree.RootPhrase(_settings.Hundred);
            advanced = 0;
            Hundred = false;
            return true;
        }

        switch (PositionalIndex)
        {
            case 3:
                ProcessThousand(out phrase, out advanced);
                return true;
            case 2:
                ProcessHundred(out phrase, out advanced);
                return true;
            case 1:
                ProcessTen(out phrase, out advanced);
                return true;
            case 0:
                phrase = _tree.Digit(Digit, Interpretation == NumberInterpretation.Ordinal ? _settings.OneOrdinal : _settings.OneExact);
                advanced = 1;
                PositionalIndex = -1;
                return true;
            default:
                phrase = default;
                advanced = 0;
                return false;
        }
    }

    private void ProcessHundred(out UtteranceSegment<T> phrase, out int advanced)
    {
        var hundredNow = !_settings.ExplicitOne && _text[^3] == '1';
        var zeroes = _text.Count('0', ^2);
        phrase = hundredNow ? _tree.RootPhrase(_settings.Hundred) : _tree.Digit(_text[^3], _settings.OneExact);
        advanced = 1 + zeroes;
        PositionalIndex -= advanced;
        Hundred = !hundredNow;
    }

    private void ProcessThousand(out UtteranceSegment<T> phrase, out int advanced)
    {
        var thousandNow = !_settings.ExplicitOne && _text[^4] == '1';
        var zeroes = _text.Count('0', ^3);
        phrase = thousandNow ? _tree.RootPhrase(_settings.Thousand) : _tree.Digit(_text[^4], _settings.OneExact);
        advanced = 1 + zeroes;
        PositionalIndex -= advanced;
        Thousand = !thousandNow;
    }

    private void ProcessTen(out UtteranceSegment<T> phrase, out int advanced)
    {
        if (_text[^1] == '0')
        {
            phrase = _tree.Digit(_text[^2], Interpretation == NumberInterpretation.Ordinal ? _settings.TenOrdinal : _settings.TenExact);
            advanced = 2;
            PositionalIndex = -1;
            return;
        }

        phrase = _tree.Digit(Digit, _settings.Ten);
        advanced = 1;
        PositionalIndex = 0;
    }

    private bool NextDigit(out UtteranceSegment<T> phrase, out int advanced)
    {
        phrase = _tree.Digit(Digit, _settings.OneExact);
        advanced = 1;
        PositionalIndex--;
        return true;
    }

    public static SequentialNumberParser<T> CreateTrimmed(ReadOnlySpan<char> text, PhraseTree<T> tree, NumberSettings mappers, NumberInterpretation interpretation, out int advanced)
    {
        var trimmed = text.TrimStart('0');
        if (trimmed.IsEmpty)
            trimmed = "0";
        advanced = text.Length - trimmed.Length;
        return new SequentialNumberParser<T>(trimmed, tree, mappers, interpretation);
    }

}
