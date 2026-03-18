using System;
using UnityEngine;

[Serializable]
public class ItemData
{
    [Header("기본 정보")]
    public string ItemId;
    public string ItemName;
    [TextArea]
    public string Description;

    [Header("표시")]
    public Sprite Icon;
    public GameObject Prefab;


}
