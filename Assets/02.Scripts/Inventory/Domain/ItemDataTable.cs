using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataTable", menuName = "Inventory/ItemDataTable")]
public class ItemDataTable : ScriptableObject
{
    [SerializeField]private List<ItemData> _itemDatas = new();

    public ItemData GetItemData(int itemId)
    {
        ItemData data = _itemDatas.Find(item => item.ItemId == itemId);
        if (data == null)
        {
            Debug.LogWarning($"Item {itemId} not found");
            return null;
        }
        
        return data;
    }

    public ItemInstance CreateInstance(int itemId)
    {
        ItemData definition = GetItemData(itemId);
        if (definition == null)
        {
            Debug.LogWarning($"Item {itemId} not found");
            return null;
        }

        return new ItemInstance(definition);
    }
}
