using UnityEngine;

public class WorldViewService
{
    private readonly ItemFactory _itemFactory;

    public WorldViewService(ItemFactory itemFactory)
    {
        _itemFactory = itemFactory;
    }

    public GameObject Create(RuntimeItemData runtimeItemData, Transform root)
    {
        GameObject itemObject = _itemFactory.CreateWorldObject(runtimeItemData, root);
        if (itemObject == null)
        {
            return null;
        }

        if (root != null)
        {
            itemObject.transform.localPosition = Vector3.zero;
            _itemFactory.SetLayerRecursively(itemObject, root.gameObject.layer);
        }

        if (itemObject.TryGetComponent(out IRuntimeView runtimeView))
        {
            runtimeView.Bind(runtimeItemData);
            runtimeView.RefreshView();
        }

        return itemObject;
    }
}