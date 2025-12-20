namespace Katie.SecretLab;

[Serializable]
public sealed class KatieConfig
{

    public bool ReplaceCassie { get; set; }

    public string DefaultLanguage { get; set; } = "English";

    public string? DefaultSignal { get; set; }

}
