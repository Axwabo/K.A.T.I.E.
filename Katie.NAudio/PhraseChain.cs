using System;
using System.Collections.Generic;
using Katie.Core;
using Katie.Core.DataStructures;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Katie.NAudio;

public static class PhraseChain
{

    public static WaveFormat Format { get; } = WaveFormat.CreateIeeeFloatWaveFormat(48000, 1);

    public static ISampleProvider? Parse(ReadOnlySpan<char> text, PhraseTree<WavePhrase> tree)
    {
        var providers = new List<ISampleProvider>();
        var parser = new PhraseParser<WavePhrase>(text, tree);
        while (parser.Next(out var phrase))
            switch (phrase)
            {
                case {Phrase: { } clip}:
                    providers.Add(clip.ToSampleProvider());
                    break;
                case {Duration.TotalSeconds: not 0 and var seconds}:
                    providers.Add(new DurationSilenceSampleProvider(Format, seconds));
                    break;
            }

        return providers.Count == 0 ? null : new ConcatenatingSampleProvider(providers);
    }

}
