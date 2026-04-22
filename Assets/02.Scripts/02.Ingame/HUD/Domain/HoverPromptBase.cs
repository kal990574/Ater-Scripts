using UnityEngine;

public class HoverPromptBase : MonoBehaviour
{
    [SerializeField] private int[] _defaultPromptIds;
    [SerializeField] private int[] _activePromptIds;
    [SerializeField] private int[] _unlockedPromptIds;

    private IDetectable _detectable;
    private bool _isHovering;
    private bool _isScanComplete;
    private bool _isUnlocked;

    private void Awake()
    {
        _detectable = GetComponent<IDetectable>();
        if (_detectable != null)
            _detectable.OnDetected += HandleDetected;
    }
    private void OnDisable()
    {
        if (!_isHovering) return;
        _isHovering = false;
        HoverPromptManager.Instance?.HidePrompt();
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
        RefreshIfHovering();
    }

    public void OnHoverExit()
    {
        _isHovering = false;
        HoverPromptManager.Instance?.HidePrompt();
    }
    public void OnScanComplete()
    {
        _isScanComplete = true;
        RefreshIfHovering();
    }
    public void Unlocked()
    {
        _isUnlocked = true;
        RefreshIfHovering();

    }
    private void RefreshIfHovering()
    {
        if (!_isHovering) return;
        HoverPromptManager.Instance?.ShowPrompt(GetCurrentPromptIds());
    }
    private int[] GetCurrentPromptIds()
    {
        if (_isUnlocked && HasIds(_unlockedPromptIds)) return _unlockedPromptIds;
        if (_isScanComplete && HasIds(_activePromptIds)) return _activePromptIds;
        return _defaultPromptIds;
    }
    private static bool HasIds(int[] ids) => ids != null && ids.Length > 0;
}