using System;
using UnityEngine;

public class ItemFactory
{
    private readonly RuntimeInstanceManager _runtimeInstanceManager;

    public ItemFactory(RuntimeInstanceManager runtimeInstanceManager)
    {
        _runtimeInstanceManager = runtimeInstanceManager;
    }

    public GameObject CreateExamineObject(RuntimeItemData runtimeItemData, Transform parent)
    {
        return CreateBoundObject(runtimeItemData, parent, item => item.ExaminePrefab);
    }

    public GameObject CreateHandObject(RuntimeItemData runtimeItemData, Transform parent)
    {
        return CreateBoundObject(runtimeItemData, parent, item => item.HandPrefab);
    }

    public GameObject CreateWorldObject(RuntimeItemData runtimeItemData, Transform parent)
    {
        return CreateBoundObject(runtimeItemData, parent, item => item.WorldPrefab);
    }

    public void Bind(GameObject itemObject, RuntimeItemData runtimeItemData)
    {
        if (itemObject == null || runtimeItemData == null)
        {
            return;
        }

        if (!itemObject.TryGetComponent(out IRuntimeView runtimeView))
        {
            Debug.Log($"[{nameof(ItemFactory)}] Can't find {nameof(IRuntimeView)}.");
            return;
        }

        runtimeView.Bind(runtimeItemData);
    }

    public void SetLayerRecursively(GameObject obj, int layer)
    {
        if (obj == null)
        {
            return;
        }

        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private GameObject CreateBoundObject(RuntimeItemData runtimeItemData, Transform parent, Func<RuntimeItemData, GameObject> prefabSelector)
    {
        if (runtimeItemData == null)
        {
            return null;
        }

        GameObject prefab = prefabSelector(runtimeItemData);
        if (prefab == null)
        {
            return null;
        }

        GameObject itemObject = UnityEngine.Object.Instantiate(prefab, parent, false);
        Bind(itemObject, runtimeItemData);
        return itemObject;
    }
}