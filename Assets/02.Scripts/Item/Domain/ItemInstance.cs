using System;
using UnityEngine;

//런타임에서 생성되는 아이템
//현재 인스턴스의 상태 보관
[Serializable]
public class ItemInstance
{
    [SerializeField] private string _instanceId;
    [SerializeField] private ItemData _definition;
    [SerializeField] private ItemState _state;

    public string InstanceId => _instanceId;
    public ItemData Definition => _definition;
    public ItemState State => _state;

    public int ItemId => _definition != null ? _definition.ItemId : -1;
    public string ItemName => _definition != null ? _definition.ItemName : string.Empty;
    public string Description => _definition != null ? _definition.Description : string.Empty;
    public Sprite Icon => _definition != null ? _definition.Icon : null;
    public GameObject Prefab => _definition != null ? _definition.Prefab : null;

    public ItemInstance(ItemData definition)
    {
        _instanceId = Guid.NewGuid().ToString("N");
        _definition = definition;
        _state = definition != null ? definition.CreateDefaultState() : new ItemState();
    }
}
