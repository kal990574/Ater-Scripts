using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(RuntimeView))]
public class RuntimeItemDataInitializer : MonoBehaviour
{
    [SerializeField] private string _initialId;
    [SerializeField] private int _itemId = -1;

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
        InventoryManager inventoryManager = InventoryManager.Instance;
        if (inventoryManager == null)
        {
            Debug.LogError($"[{nameof(RuntimeItemDataInitializer)}] {nameof(InventoryManager)}.Instance is null.", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(_initialId))
        {
            Debug.LogError($"[{nameof(RuntimeItemDataInitializer)}] Initial ID is missing.", this);
            return;
        }

        if (_itemId < 0)
        {
            Debug.LogError($"[{nameof(RuntimeItemDataInitializer)}] Item ID is invalid.", this);
            return;
        }

        RuntimeItemData runtimeItemData = inventoryManager.GetOrCreateItemInstance(_initialId, _itemId);
        if (runtimeItemData == null)
        {
            return;
        }

        _runtimeView.Bind(runtimeItemData.InstanceId, inventoryManager);
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
