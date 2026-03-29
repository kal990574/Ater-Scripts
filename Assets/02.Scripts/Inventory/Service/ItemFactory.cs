using UnityEngine;

public class ItemFactory
{
    private readonly RuntimeInstanceService _runtimeInstanceService;
    private readonly InventoryManager _inventoryManager;

    public ItemFactory(RuntimeInstanceService runtimeInstanceService, InventoryManager inventoryManager)
    {
        _runtimeInstanceService = runtimeInstanceService;
        _inventoryManager = inventoryManager;
    }

    public GameObject CreateExamineObject(string instanceId, Transform parent)
    {
        return CreateBoundObject(instanceId, parent, item => item.ExaminePrefab);
    }

    public GameObject CreateHandObject(string instanceId, Transform parent)
    {
        return CreateBoundObject(instanceId, parent, item => item.HandPrefab);
    }

    public GameObject CreateWorldObject(string instanceId, Transform parent)
    {
        return CreateBoundObject(instanceId, parent, item => item.WorldPrefab);
    }

    public void Bind(GameObject itemObject, string instanceId)
    {
        if (itemObject == null || string.IsNullOrEmpty(instanceId))
        {
            return;
        }

        if (!itemObject.TryGetComponent(out IItemInstance binder))
        {
            Debug.Log("[ItemFactory] Can't find IItemInstance binder.");
            return;
        }

        binder.Bind(instanceId, _inventoryManager);
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

    private GameObject CreateBoundObject(string instanceId, Transform parent, System.Func<InstanceData, GameObject> prefabSelector)
    {
        if (!_runtimeInstanceService.TryGetInstance(instanceId, out InstanceData itemInstanceData) || itemInstanceData == null)
        {
            return null;
        }

        GameObject prefab = prefabSelector(itemInstanceData);
        if (prefab == null)
        {
            return null;
        }

        GameObject itemObject = Object.Instantiate(prefab, parent, false);
        Bind(itemObject, instanceId);
        return itemObject;
    }
}
