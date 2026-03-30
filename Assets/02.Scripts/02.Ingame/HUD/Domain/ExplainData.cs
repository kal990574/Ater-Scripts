using System;
using System.Globalization;
using UnityEngine;

[Serializable]
public class ExplainData
{
    [SerializeField]private string _explainId;
    [SerializeField][TextArea] private string _explainText;

    public string Explainid => _explainId;
    public string ExplainText => _explainText;
}
