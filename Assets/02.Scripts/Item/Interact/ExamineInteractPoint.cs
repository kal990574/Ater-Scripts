using UnityEngine;
using UnityEngine.Events;

public class ExamineInteractPoint : MonoBehaviour
{
    public UnityEvent OnInteract;
    
    public void OnClick()
    {
        OnInteract?.Invoke();
    }
    
}
