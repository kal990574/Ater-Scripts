using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(RuntimeView))]
public class RuntimeItemDataInitializer : MonoBehaviour
{
    [SerializeField] private string _initialId;
    [SerializeField] private string _initialName;
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
        RuntimeInstanceManager runtimeInstanceManager = RuntimeInstanceManager.Instance;
        if (runtimeInstanceManager == null)
        {
            Debug.LogError($"[{nameof(RuntimeItemDataInitializer)}] {nameof(RuntimeInstanceManager)}.Instance is null.", this);
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

        string objectName = ResolveItemObjectName(runtimeInstanceManager);
        RuntimeItemData runtimeItemData = runtimeInstanceManager.GetOrCreateItemInstance(_initialId, _itemId, objectName);
        if (runtimeItemData == null)
        {
            return;
        }

        _runtimeView.Bind(runtimeItemData);
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

    private string ResolveItemObjectName(RuntimeInstanceManager runtimeInstanceManager)
    {
        if (runtimeInstanceManager == null)
        {
            return _initialName;
        }

        ItemData itemData = runtimeInstanceManager.ItemDataTable.GetItemData(_itemId);
        if (itemData == null || string.IsNullOrWhiteSpace(itemData.ItemName))
        {
            return _initialName;
        }

        return itemData.ItemName;
    }
}
