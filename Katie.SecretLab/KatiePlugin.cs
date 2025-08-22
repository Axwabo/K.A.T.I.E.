using HarmonyLib;
using LabApi.Events.CustomHandlers;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;

namespace Katie.SecretLab;

public sealed class KatiePlugin : Plugin
{

    public override string Name => "K.A.T.I.E. TTS";
    public override string Description => "Custom announcer";
    public override string Author => "Axwabo";
    public override Version Version => GetType().Assembly.GetName().Version;
    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    private readonly Harmony _harmony = new("Katie.SecretLab");

    private readonly EventHandlers _handlers = new();

    public override void Enable()
    {
        _harmony.PatchAll();
        var config = this.GetConfigDirectory();
        PhraseCache.Initialize(config);
        CustomHandlersManager.RegisterEventsHandler(_handlers);
    }

    public override void Disable()
    {
        _harmony.UnpatchAll(_harmony.Id);
        CustomHandlersManager.UnregisterEventsHandler(_handlers);
    }

}
