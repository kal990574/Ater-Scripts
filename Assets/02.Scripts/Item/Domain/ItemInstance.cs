using System;
using UnityEngine;

//런타임에서 생성되는 아이템
//현재 인스턴스의 상태 보관
[Serializable]
public class ItemInstance
{
    [SerializeField] private string _instanceId;
    [SerializeField] private ItemData data;
    [SerializeField] private ItemState _state;

    public string InstanceId => _instanceId;
    public ItemData Data => data;
    public ItemState State => _state;

    public int ItemId => data != null ? data.ItemId : -1;
    public string ItemName => data != null ? data.ItemName : string.Empty;
    public string Description => data != null ? data.Description : string.Empty;
    public Sprite Icon => data != null ? data.Icon : null;
    public GameObject WorldPrefab => data != null ? data.WorldPrefab : null;
    public GameObject ExaminePrefab => data != null ? data.ExaminePrefab : null;
    public GameObject HandPrefab => data != null ? data.HandPrefab : null;

    //데이터로 인스턴스 제작하기
    public ItemInstance(ItemData data = null)
    {
        _instanceId = Guid.NewGuid().ToString("N");
        this.data = data;
        _state = data != null ? data.CreateDefaultState() : new ItemState();
    }
}
