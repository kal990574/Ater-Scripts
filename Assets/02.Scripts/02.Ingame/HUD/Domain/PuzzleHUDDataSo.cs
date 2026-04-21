using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleHUDDataSO", menuName = "Ater/HUD/PuzzleHUDData")]
public class PuzzleHUDDataSo : ScriptableObject
{
    [SerializeField] private List<PuzzleHUDData> _datas;

    public bool BuildPuzzleHUDString(EPuzzleType type, out string result)
    {
        var data = _datas.FindAll(d => d.PuzzleType == type);
        if (data == null || data.Count == 0)
        {
            result = string.Empty;
            return false;
        }

        var sb = new StringBuilder();
        foreach (var entry in data)
        {
            if (sb.Length > 0) sb.Append("\n\n");
            sb.Append($"<sprite name={entry.Icon}> : {entry.Label}");
        }
        result = sb.ToString();
        return true;
    }

}