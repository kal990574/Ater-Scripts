using System;
using UnityEngine;

[Serializable]
public class RuntimeData
{
    [SerializeField] private string _instanceId;
    [SerializeField] private InteractState _state;

    public string InstanceId => _instanceId;
    public InteractState State => _state;

    public RuntimeData(InteractState initialState = null)
        : this(null, initialState)
    {
    }

    public RuntimeData(string instanceId, InteractState initialState = null)
    {
        _instanceId = string.IsNullOrWhiteSpace(instanceId) ? Guid.NewGuid().ToString("N") : instanceId;
        _state = initialState != null ? initialState.Clone() : new InteractState();
    }
}
