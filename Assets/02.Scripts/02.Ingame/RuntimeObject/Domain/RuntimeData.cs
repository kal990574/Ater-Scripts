using System;
using UnityEngine;

[Serializable]
public class RuntimeData
{
    [SerializeField] private string _instanceId;
    [SerializeField] private string _objectName;
    [SerializeField] private InteractState _state;

    public string InstanceId => _instanceId;
    public string ObjectName => _objectName;
    public InteractState State => _state;

    public RuntimeData(InteractState initialState = null)
        : this(null, null, initialState)
    {
    }

    public RuntimeData(string instanceId, InteractState initialState = null)
        : this(instanceId, null, initialState)
    {
    }

    public RuntimeData(string instanceId, string objectName, InteractState initialState = null)
    {
        _instanceId = string.IsNullOrWhiteSpace(instanceId) ? Guid.NewGuid().ToString("N") : instanceId;
        _objectName = objectName ?? string.Empty;
        _state = initialState != null ? initialState.Clone() : new InteractState();
    }
}
