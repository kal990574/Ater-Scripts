using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataTable", menuName = "Inventory/ItemDataTable")]
public class ItemDataTable : ScriptableObject
{
    [SerializeField]private List<ItemData> _itemDatas = new();

    public ItemData GetItem(int itemId)
    {
        ItemData origin = _itemDatas.Find(item => item.ItemId == itemId);
        if (origin == null) return null;

        return origin.Clone();
    }

    public ItemInstance CreateInstance(int itemId)
    {
        ItemData definition = GetItem(itemId);
        if (definition == null)
        {
            return null;
        }

        return new ItemInstance(definition);
    }
}
