using _02.Scripts.AIHint.Domain.Models;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExplainDataTable", menuName = "Ater/HUD/ExplainDataTable")]
public class ExplainDataSO : ScriptableObject
{
    [SerializeField] private List<ExplainData> _explains;

    public ExplainData GetExplainData(string explainid)
    {
        ExplainData data = _explains.Find(explain => explain.Explainid == explainid);

        if (data == null)
        {
            Debug.LogWarning($"Item {explainid} not found");
            return null;
        }

        return data;
    }
}

