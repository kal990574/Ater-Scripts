using System;
using UnityEngine;

[Serializable]
public class TensionRule
{
    [SerializeField] private string _reason;
    [SerializeField] private ETensionChannel _channel = ETensionChannel.BaseTension;
    [SerializeField] private float _delta = 0f;

    public string Reason => _reason;
    public ETensionChannel Channel => _channel;
    public float Delta => _delta;

    public void SetReason(string reason)
    {
        _reason = reason;
    }
}