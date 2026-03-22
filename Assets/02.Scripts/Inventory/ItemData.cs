using System;
using UnityEngine;

[Serializable]
public class ItemData
{
    [Header("기본 정보")]
    [SerializeField] private int _itemId;
    [SerializeField] private string _itemName;
    [SerializeField] private string _description;

    [Header("표시")]
    [SerializeField] private Sprite _icon;
    [SerializeField] private GameObject _prefab;

    public int ItemId => _itemId;
    public string ItemName => _itemName;
    public string Description => _description;
    public Sprite Icon => _icon;
    public GameObject Prefab => _prefab;
    public ItemData Clone()
    {
        ItemData clone = new ItemData();
        clone._itemId = _itemId;
        clone._itemName = _itemName;
        clone._description = _description;
        clone._icon = _icon;
        clone._prefab = _prefab;
        return clone;
    }

}
