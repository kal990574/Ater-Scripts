using UnityEngine;
public class HoverOutLine : MonoBehaviour, IHoverable
{
    [SerializeField] private InteractTargetShaderModifier _shaderPropertyController;
    
    [ContextMenu("hover")]
    public void OnHoverEnter()
    {
        _shaderPropertyController.SetOutlineThickness(true);
    }

    [ContextMenu("unhover")]
    public void OnHoverExit()
    {
        _shaderPropertyController.SetOutlineThickness(false);
    }
}
