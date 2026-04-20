using UnityEngine;

public class HoverPromptBase : MonoBehaviour
{
    [SerializeField] private int[] _promptIds;

    public void OnHoverEnter() => HoverPromptManager.instance?.ShowPrompt(_promptIds);
    public void OnHoverExit() => HoverPromptManager.instance?.HidePrompt();

}