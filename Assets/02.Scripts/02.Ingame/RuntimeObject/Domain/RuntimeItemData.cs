using System;
using UnityEngine;

[Serializable]
public class RuntimeItemData : RuntimeData
{
    [SerializeField] private ItemData data;

    public ItemData Data => data;
    public int ItemId => data != null ? data.ItemId : -1;
    public string ItemName => data != null ? data.ItemName : string.Empty;
    public EItemType ItemType => data != null ? data.ItemType : EItemType.None;
    public string Description => data != null ? data.Description : string.Empty;
    public Sprite Icon => data != null ? data.Icon : null;
    public GameObject WorldPrefab => data != null ? data.WorldPrefab : null;
    public GameObject ExaminePrefab => data != null ? data.ExaminePrefab : null;
    public GameObject HandPrefab => data != null ? data.HandPrefab : null;

    public RuntimeItemData(ItemData data = null)
        : this(null, null, data)
    {
    }

    public RuntimeItemData(string instanceId, ItemData data = null)
        : this(instanceId, null, data)
    {
    }

    public RuntimeItemData(string instanceId, string objectName, ItemData data = null)
        : base(instanceId, objectName, data != null ? data.CreateDefaultState() : null)
    {
        this.data = data;
    }
}
