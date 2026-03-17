using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataTable", menuName = "Inventory/ItemDataTable")]
public class ItemDataTable : ScriptableObject
{
    public List<ItemData> ItemDatas = new();

    public ItemData GetItem(string itemId)
    {
        return ItemDatas.Find(item => item.ItemId == itemId);
    }
}
