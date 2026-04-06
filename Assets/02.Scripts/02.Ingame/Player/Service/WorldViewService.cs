using UnityEngine;

public class WorldViewService
{
    private readonly RuntimeItemFactory runtimeItemFactory;

    public WorldViewService(RuntimeItemFactory runtimeItemFactory)
    {
        this.runtimeItemFactory = runtimeItemFactory;
    }

    public GameObject Create(RuntimeItemData runtimeItemData, Transform root)
    {
        GameObject itemObject = runtimeItemFactory.CreateWorldObject(runtimeItemData, root);
        if (itemObject == null)
        {
            return null;
        }

        if (root != null)
        {
            itemObject.transform.localPosition = Vector3.zero;
            runtimeItemFactory.SetLayerRecursively(itemObject, root.gameObject.layer);
        }

        if (itemObject.TryGetComponent(out IRuntimeView runtimeView))
        {
            runtimeView.Bind(runtimeItemData);
            runtimeView.RefreshView();
        }

        return itemObject;
    }
}