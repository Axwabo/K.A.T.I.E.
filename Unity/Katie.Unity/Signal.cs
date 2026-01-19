using UnityEngine;

namespace Katie.Unity;

[CreateAssetMenu(fileName = "Signal", menuName = "K.A.T.I.E./Signal", order = 0)]
public sealed class Signal : ScriptableObject
{

    [field: SerializeField]
    public AudioClip Clip { get; private set; } = null!;
    
    [field: SerializeField]
    public float Duration { get; private set; }

}
