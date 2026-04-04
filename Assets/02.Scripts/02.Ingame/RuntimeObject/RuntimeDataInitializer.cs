using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(RuntimeView))]
public class RuntimeDataInitializer : MonoBehaviour
{
    [SerializeField] private string _initialId;
    [SerializeField] private InteractState _defaultState = new();

    private RuntimeView _runtimeView;

    private void Awake()
    {
        EnsureInitialId();
        _runtimeView = GetComponent<RuntimeView>();
        SyncRuntimeView();
    }

    private void Reset()
    {
        EnsureInitialId();
        SyncRuntimeView();
    }

    private void OnValidate()
    {
        EnsureInitialId();
        SyncRuntimeView();
    }

    private void Start()
    {
        RuntimeInstanceManager runtimeInstanceManager = RuntimeInstanceManager.Instance;
        if (runtimeInstanceManager == null)
        {
            Debug.LogError($"[{nameof(RuntimeDataInitializer)}] {nameof(RuntimeInstanceManager)}.Instance is null.", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(_initialId))
        {
            Debug.LogError($"[{nameof(RuntimeDataInitializer)}] Initial ID is missing.", this);
            return;
        }

        RuntimeData runtimeData = runtimeInstanceManager.GetRuntimeData(_initialId);
        if (runtimeData == null)
        {
            runtimeData = runtimeInstanceManager.GetOrCreateRuntimeData(_initialId, _defaultState);
        }

        if (runtimeData == null)
        {
            return;
        }

        _runtimeView.Bind(runtimeData);
    }

    private void EnsureInitialId()
    {
        if (!string.IsNullOrWhiteSpace(_initialId))
        {
            return;
        }

        _initialId = Guid.NewGuid().ToString("N");
    }

    private void SyncRuntimeView()
    {
        if (_runtimeView == null)
        {
            _runtimeView = GetComponent<RuntimeView>();
        }

        if (_runtimeView == null)
        {
            return;
        }

        _runtimeView.SetInstanceId(_initialId);
    }
}