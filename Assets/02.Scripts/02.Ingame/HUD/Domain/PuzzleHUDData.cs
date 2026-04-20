using System;
using UnityEngine;

[Serializable]
public class PuzzleHUDData
{
    [SerializeField] private EPuzzleType _puzzleType;
    [SerializeField] private string _icon;
    [SerializeField] private string _label;

    public EPuzzleType PuzzleType => _puzzleType;
    public string Icon => _icon;
    public string Label => _label;

}