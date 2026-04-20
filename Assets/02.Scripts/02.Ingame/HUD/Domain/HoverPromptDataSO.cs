using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "HoverPromptDataSO", menuName = "Ater/HUD/HoverPromptData")]
public class HoverPromptDataSO : ScriptableObject
{
    [SerializeField] private List<HoverPromptData> _datas;

    public string BuildPromptString(int[] ids)
    {
        if(ids == null || ids.Length == 0)
            return string.Empty;

        var sb = new StringBuilder();
        foreach(int id in ids )
        {
            HoverPromptData data = _datas.Find(d => d.Id ==  id);
            if(data ==  null) continue;
            if(sb.Length > 0 ) sb.Append(" ");
            sb.Append($"<sprite name={data.Icon}> : {data.Label}");
        }
        return sb.ToString();
    }

}