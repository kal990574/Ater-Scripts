using System;
using UnityEngine;

[Serializable]

public class ItemData
{
    [Header("기본 정보")]
    [SerializeField] private int _itemId;
    [SerializeField] private string _itemName;
    [SerializeField] private string _description;
    [SerializeField] private EItemType _itemType;
    [Header("표시")]
    [SerializeField] private Sprite _icon;
    [SerializeField] private GameObject _worldPrefab;
    [SerializeField] private GameObject _examinePrefab;
    [SerializeField] private GameObject _handPrefab;
    
    [Header("생성시 지정할 State")]
    [SerializeField] private InteractState _defaultState = new();

    public int ItemId => _itemId;
    public string ItemName => _itemName;
    public string Description => _description;
    public EItemType ItemType => _itemType;
    public Sprite Icon => _icon;
    public GameObject WorldPrefab => _worldPrefab;
    public GameObject ExaminePrefab => _examinePrefab;
    public GameObject HandPrefab => _handPrefab;

    public InteractState CreateDefaultState()
    {
        return _defaultState != null ? _defaultState.Clone() : new InteractState();
    }

}
