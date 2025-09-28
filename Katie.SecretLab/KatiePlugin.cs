using LabApi.Events.CustomHandlers;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using Mirror;
using Mirror.RemoteCalls;
using Respawning;

namespace Katie.SecretLab;

public sealed class KatiePlugin : Plugin<KatieConfig>
{

    private const string ClearQueue = "System.Void Respawning.RespawnEffectsController::RpcClearQueue()";

    public static KatiePlugin Instance { get; private set; } = null!;

    public override string Name => "K.A.T.I.E. TTS";
    public override string Description => "Custom announcer";
    public override string Author => "Axwabo";
    public override Version Version => GetType().Assembly.GetName().Version;
    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    private readonly EventHandlers _handlers = new();

    public override void Enable()
    {
        Instance = this;
        var config = this.GetConfigDirectory();
        PhraseCache.Initialize(config);
        CustomHandlersManager.RegisterEventsHandler(_handlers);
        _ = RespawnEffectsController.AllControllers; // invoke static ctor
        var hash = (ushort) (ClearQueue.GetStableHashCode() & ushort.MaxValue);
        RemoteProcedureCalls.RemoveDelegate(hash);
        RemoteProcedureCalls.RegisterRpc(typeof(RespawnEffectsController), ClearQueue, (_, _, _) =>
        {
            KatieAnnouncer.Stop();
            NineTailedFoxAnnouncer.singleton.ClearQueue();
        });
    }

    public override void Disable() => CustomHandlersManager.UnregisterEventsHandler(_handlers);

}
