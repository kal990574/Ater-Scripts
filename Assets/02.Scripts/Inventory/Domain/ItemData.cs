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
    [SerializeField] private GameObject _handPrefab;
    [SerializeField] private ItemState _defaultState = new();

    public int ItemId => _itemId;
    public string ItemName => _itemName;
    public string Description => _description;
    public Sprite Icon => _icon;
    public GameObject Prefab => _prefab;
    public GameObject HandPrefab => _handPrefab != null ? _handPrefab : _prefab;

    public ItemData Clone()
    {
        ItemData clone = new ItemData();
        clone._itemId = _itemId;
        clone._itemName = _itemName;
        clone._description = _description;
        clone._icon = _icon;
        clone._prefab = _prefab;
        clone._handPrefab = _handPrefab;
        clone._defaultState = _defaultState != null ? _defaultState.Clone() : new ItemState();
        return clone;
    }

    public ItemState CreateDefaultState()
    {
        return _defaultState != null ? _defaultState.Clone() : new ItemState();
    }

}
