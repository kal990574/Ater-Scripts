using UnityEngine;

public interface IQTEInvoker
{
    float CurrentQTEDelay { get; }
    void SetQTEDelay();
    void HandleQteFailure();
    
}
