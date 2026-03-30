using UnityEngine;

public class WorldViewService
{
    private readonly ItemFactory _itemFactory;

    public WorldViewService(ItemFactory itemFactory)
    {
        _itemFactory = itemFactory;
    }

    public GameObject Create(string instanceId, Transform root)
    {
        GameObject itemObject = _itemFactory.CreateWorldObject(instanceId, root);
        if (itemObject == null)
        {
            return null;
        }

        if (root != null)
        {
            itemObject.transform.localPosition = Vector3.zero;
            _itemFactory.SetLayerRecursively(itemObject, root.gameObject.layer);
        }

        return itemObject;
    }
}
