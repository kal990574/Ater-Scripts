using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SubJumpScareDefinitionSO : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string id;
    [SerializeField] private ESubJumpScareType type = ESubJumpScareType.None;

    [Header("Tension Range")]
    [SerializeField] private float minTension = 0.0f;
    [SerializeField] private float maxTension = 100.0f;

    [Header("Selection")]
    [SerializeField] private int priority = 0;
    [SerializeField] private float weight = 1.0f;
    [SerializeField] private float cooldownSeconds = 10.0f;

    [Header("Allowed Modes")]
    [SerializeField] private List<EPlayerInteractMode> allowedModes = new List<EPlayerInteractMode>()
    {
        EPlayerInteractMode.Scan,
        EPlayerInteractMode.Item
    };

    [Header("Optional Constraints")]
    [SerializeField] private bool blockWhenMainJumpScareRunning = true;
    [SerializeField] private bool blockWhenSubJumpScareRunning = true;

    public string Id => id;
    public ESubJumpScareType Type => type;
    public float MinTension => minTension;
    public float MaxTension => maxTension;
    public int Priority => priority;
    public float Weight => weight;
    public float CooldownSeconds => cooldownSeconds;
    public bool BlockWhenMainJumpScareRunning => blockWhenMainJumpScareRunning;
    public bool BlockWhenSubJumpScareRunning => blockWhenSubJumpScareRunning;

    public bool IsInTensionRange(float tension)
    {
        return tension >= minTension && tension <= maxTension;
    }

    public bool IsAllowedMode(EPlayerInteractMode playerMode)
    {
        for (int index = 0; index < allowedModes.Count; index++)
        {
            if (allowedModes[index] == playerMode)
            {
                return true;
            }
        }

        return false;
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        minTension = Mathf.Max(0.0f, minTension);
        maxTension = Mathf.Max(minTension, maxTension);
        weight = Mathf.Max(0.0f, weight);
        cooldownSeconds = Mathf.Max(0.0f, cooldownSeconds);
    }
#endif
}