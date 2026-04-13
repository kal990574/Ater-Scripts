using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionFailSO", menuName = "UI/Interaction Fail SO")]
public class InteractionFailSO : ScriptableObject
{
    [SerializeField] private List<string> _message;
    public bool TryGetMessage(int index, out string message)
    {
        if (_message == null || index < 0 || index >= _message.Count)
        {
            message = null;
            return false;
        }
        message = _message[index];
        return true;
    }
}
