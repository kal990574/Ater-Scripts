using System;
using UnityEngine;

[Serializable]
public class HoverPromptData
{
   [SerializeField] private int _id;
   [SerializeField] private string _icon;
   [SerializeField] private string _label;

    public int Id => _id;
    public string Icon => _icon;
    public string Label => _label;

}
