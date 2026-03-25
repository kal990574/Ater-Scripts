using System;
using UnityEngine;

[Serializable]
public class InventoryItemInstance
{
    [SerializeField] private string _instanceId;
    [SerializeField] private ItemData _definition;
    [SerializeField] private InventoryItemState _state;

    public string InstanceId => _instanceId;
    public ItemData Definition => _definition;
    public InventoryItemState State => _state;

    public int ItemId => _definition != null ? _definition.ItemId : -1;
    public string ItemName => _definition != null ? _definition.ItemName : string.Empty;
    public string Description => _definition != null ? _definition.Description : string.Empty;
    public Sprite Icon => _definition != null ? _definition.Icon : null;
    public GameObject Prefab => _definition != null ? _definition.Prefab : null;

    public InventoryItemInstance(ItemData definition)
    {
        _instanceId = Guid.NewGuid().ToString("N");
        _definition = definition;
        _state = definition != null ? definition.CreateDefaultState() : new InventoryItemState();
    }
}
