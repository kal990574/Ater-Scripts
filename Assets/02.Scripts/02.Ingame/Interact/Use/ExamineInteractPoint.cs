using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class ExamineInteractPoint : MonoBehaviour
{
    [TabGroup("Inspector", "ExamineInteractPoint")]
    [LabelText("On Interact")]
    public UnityEvent OnInteract;
    
    public void OnClick()
    {
        OnInteract?.Invoke();
    }
    
}
