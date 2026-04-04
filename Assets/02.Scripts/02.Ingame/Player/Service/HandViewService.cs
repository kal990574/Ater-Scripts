using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class HandViewService
{
    private readonly Dictionary<int, GameObject> _cache = new Dictionary<int, GameObject>();
    private readonly RuntimeItemFactory runtimeItemFactory;
    private readonly InventoryManager _inventoryManager;
    private readonly RuntimeInstanceManager _runtimeInstanceManager;
    private readonly Transform _root;

    private GameObject _currentObject;

    public string EquippedInstanceId { get; private set; }

    public event Action<string> OnEquippedChanged;

    public HandViewService(
        RuntimeItemFactory runtimeItemFactory,
        InventoryManager inventoryManager,
        RuntimeInstanceManager runtimeInstanceManager,
        Transform root)
    {
        this.runtimeItemFactory = runtimeItemFactory;
        _inventoryManager = inventoryManager;
        _runtimeInstanceManager = runtimeInstanceManager;
        _root = root;
    }

    public GameObject Show(int index)
    {
        Hide();

        if (!TryGetRuntimeItemData(index, out RuntimeItemData runtimeItemData))
        {
            return null;
        }

        GameObject handObject = GetOrCreate(runtimeItemData);
        if (handObject == null)
        {
            Clear();
            return null;
        }

        EquippedInstanceId = runtimeItemData.InstanceId;
        OnEquippedChanged?.Invoke(EquippedInstanceId);

        runtimeItemFactory.SetLayerRecursively(handObject, _root.gameObject.layer);
        MoveToRoot(handObject);
        handObject.SetActive(true);
        _currentObject = handObject;

        if (handObject.TryGetComponent(out IRuntimeView runtimeView))
        {
            runtimeView.Bind(runtimeItemData);
            runtimeView.RefreshView();
        }

        return handObject;
    }

    public void Hide()
    {
        if (_currentObject != null)
        {
            MoveToRoot(_currentObject);
            _currentObject.SetActive(false);
            _currentObject = null;
        }

        Clear();
    }

    public void Clear()
    {
        EquippedInstanceId = null;
        OnEquippedChanged?.Invoke(null);
    }

    public bool IsEquipped(string instanceId)
    {
        return !string.IsNullOrEmpty(instanceId) && EquippedInstanceId == instanceId;
    }

    private bool TryGetRuntimeItemData(int index, out RuntimeItemData runtimeItemData)
    {
        runtimeItemData = null;

        if (_inventoryManager == null || _runtimeInstanceManager == null)
        {
            return false;
        }

        if (!_inventoryManager.TryGetInventoryItemInstanceIdAt(index, out string instanceId))
        {
            return false;
        }

        runtimeItemData = _runtimeInstanceManager.GetItemInstance(instanceId);
        return runtimeItemData != null;
    }

    private GameObject GetOrCreate(RuntimeItemData runtimeItemData)
    {
        if (runtimeItemData == null)
        {
            return null;
        }

        int cacheKey = runtimeItemData.ItemId;
        if (_cache.TryGetValue(cacheKey, out GameObject cached) && cached != null)
        {
            runtimeItemFactory.Bind(cached, runtimeItemData);
            return cached;
        }

        GameObject created = runtimeItemFactory.CreateHandObject(runtimeItemData, _root);
        if (created == null)
        {
            return null;
        }

        created.SetActive(false);
        _cache[cacheKey] = created;
        return created;
    }

    private void MoveToRoot(GameObject itemObject)
    {
        itemObject.transform.SetParent(_root, false);
        itemObject.transform.localPosition = Vector3.zero;
        itemObject.transform.localRotation = Quaternion.identity;
        itemObject.transform.localScale = Vector3.one;
    }

    public void DisposeCache()
    {
        foreach (KeyValuePair<int, GameObject> pair in _cache)
        {
            if (pair.Value != null)
            {
                Object.Destroy(pair.Value);
            }
        }

        _cache.Clear();
        _currentObject = null;
        Clear();
    }
}