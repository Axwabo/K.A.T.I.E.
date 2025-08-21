using LabApi.Loader.Features.Plugins;

namespace Katie.SecretLab;

public sealed class KatiePlugin : Plugin
{

    public override string Name => "K.A.T.I.E.";
    public override string Description => "Custom announcer";
    public override string Author => "Axwabo";
    public override Version Version => GetType().Assembly.GetName().Version;
    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    public override void Enable()
    {
    }

    public override void Disable()
    {
    }

}
