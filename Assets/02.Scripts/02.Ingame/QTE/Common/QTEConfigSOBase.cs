using UnityEngine;

public abstract class QTEConfigSOBase : ScriptableObject
{
    [field: SerializeField] public EQTEType QTEType { get; private set; }
}