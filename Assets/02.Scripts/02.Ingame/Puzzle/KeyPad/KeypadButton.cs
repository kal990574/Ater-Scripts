using UnityEngine;
using UnityEngine.EventSystems;

public class KeypadButton : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private KeyPadPuzzleInstance _owner;
    [SerializeField] private int buttonIndex = 0;
    
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_owner == null)
        {
            return;
        }
        
        _owner.PressKey(buttonIndex);
    }
}
