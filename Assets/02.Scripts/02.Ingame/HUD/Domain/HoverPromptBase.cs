using UnityEngine;

public class HoverPromptBase : MonoBehaviour
{
    [SerializeField] private int[] _defaultPromptIds;
    [SerializeField] private int[] _activePromptIds;

    private IDetectable _detectable;
    private bool _isHovering;
    private bool _isScanComplete;

    private void Awake()
    {
        _detectable = GetComponent<IDetectable>();
        if (_detectable != null)
            _detectable.OnDetected += HandleDetected;
    }

    private void OnDestroy()
    {
        if (_detectable != null)
            _detectable.OnDetected -= HandleDetected;
    }

    private void HandleDetected(bool detected)
    {
        if (detected) OnHoverEnter();
        else OnHoverExit();
    }
    public void OnHoverEnter()
    {
        _isHovering = true;
        int[] ids = (_isScanComplete && _activePromptIds != null && _activePromptIds.Length > 0)? _activePromptIds : _defaultPromptIds;
        HoverPromptManager.instance?.ShowPrompt(ids);
    }

    public void OnHoverExit()
    {
        _isHovering = false;
        HoverPromptManager.instance?.HidePrompt();
    }
    public void OnScanComplete()
    {
        _isScanComplete = true;
        if (!_isHovering) return;
        if (_activePromptIds == null || _activePromptIds.Length == 0) return;
        HoverPromptManager.instance?.ShowPrompt(_activePromptIds);
    }

}