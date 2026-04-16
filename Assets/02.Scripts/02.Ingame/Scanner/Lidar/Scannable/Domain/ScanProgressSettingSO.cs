using UnityEngine;

[CreateAssetMenu(fileName = "ScanProgressSetting", menuName = "Ater/Scanner/Scan Progress Setting")]
public class ScanProgressSettingSO : ScriptableObject
{
    [Header("Progress")]
    [Min(0.01f)] public float MaxScanAmount = 5.0f;
    [Min(0f)] public float DecaySpeed = 2.0f;
}
