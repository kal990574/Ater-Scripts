using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class HandService
{
    public string EquippedInstanceId { get; private set; }

    public event Action<string> OnEquippedChanged;

    private readonly Dictionary<string, GameObject> _cache = new();
    private readonly ItemFactory _itemFactory;
    private readonly Transform _root;
    private GameObject _currentObject;

    public HandService(ItemFactory itemFactory, Transform root)
    {
        _itemFactory = itemFactory;
        _root = root;
    }

    public bool TryEquip(string instanceId)
    {
        if (string.IsNullOrEmpty(instanceId))
        {
            return false;
        }

        EquippedInstanceId = instanceId;
        OnEquippedChanged?.Invoke(EquippedInstanceId);
        return true;
    }

    public GameObject Show(string instanceId)
    {
        Hide();
        GameObject handObject = GetOrCreate(instanceId);
        if (handObject == null || !TryEquip(instanceId))
        {
            return null;
        }

        _itemFactory.SetLayerRecursively(handObject, _root.gameObject.layer);
        handObject.SetActive(true);
        _currentObject = handObject;
        return handObject;
    }

    public void Hide()
    {
        if (_currentObject == null)
        {
            Clear();
            return;
        }

        MoveToRoot(_currentObject);
        _currentObject.SetActive(false);
        _currentObject = null;
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

    public void Remove(string instanceId)
    {
        if (string.IsNullOrEmpty(instanceId))
        {
            return;
        }

        if (_cache.Remove(instanceId, out GameObject cached) && cached != null)
        {
            if (_currentObject == cached)
            {
                _currentObject = null;
                Clear();
            }

            Object.Destroy(cached);
        }
    }

    public void ResetState()
    {
        foreach (GameObject cached in _cache.Values)
        {
            if (cached != null)
            {
                Object.Destroy(cached);
            }
        }

        _cache.Clear();
        _currentObject = null;
        Clear();
    }

    private GameObject GetOrCreate(string instanceId)
    {
        if (string.IsNullOrEmpty(instanceId))
        {
            return null;
        }

        if (_cache.TryGetValue(instanceId, out GameObject cached) && cached != null)
        {
            _itemFactory.Bind(cached, instanceId);
            return cached;
        }

        GameObject created = _itemFactory.CreateHandObject(instanceId, _root);
        if (created == null)
        {
            return null;
        }

        created.SetActive(false);
        _cache[instanceId] = created;
        return created;
    }

    private void MoveToRoot(GameObject itemObject)
    {
        itemObject.transform.SetParent(_root, false);
        itemObject.transform.localPosition = Vector3.zero;
        itemObject.transform.localRotation = Quaternion.identity;
        itemObject.transform.localScale = Vector3.one;
    }
}
