using UnityEngine;
using UnityEngine.Events;

public class InteractPoint : MonoBehaviour
{
    public UnityEvent OnInteract;
    
    public void OnClick()
    {
        OnInteract?.Invoke();
    }
    
}
