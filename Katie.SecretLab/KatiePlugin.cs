using HarmonyLib;
using LabApi.Events.CustomHandlers;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;

namespace Katie.SecretLab;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class KatiePlugin : Plugin<KatieConfig>
{

    public static KatiePlugin Instance { get; private set; } = null!;

    public override string Name => "K.A.T.I.E. TTS";
    public override string Description => "Custom announcer";
    public override string Author => "Axwabo";
    public override Version Version => GetType().Assembly.GetName().Version;
    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    private readonly EventHandlers _handlers = new();

    private readonly Harmony _harmony = new("K.A.T.I.E.");

    public override void Enable()
    {
        Instance = this;
        var config = this.GetConfigDirectory();
        PhraseCache.Initialize(config);
        CustomHandlersManager.RegisterEventsHandler(_handlers);
        _harmony.PatchAll();
    }

    public override void Disable()
    {
        CustomHandlersManager.UnregisterEventsHandler(_handlers);
        _harmony.UnpatchAll(_harmony.Id);
    }

}
