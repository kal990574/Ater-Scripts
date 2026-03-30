using UnityEngine;

public class UI_CanvasGroupToggle : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private float _activeAlpha = 1f;
    [SerializeField] private float _inactiveAlpha = 0.4f;

    private bool _isActive = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            SetActive(!_isActive);
    }

    public void SetActive(bool active)
    {
        _isActive = active;
        _canvasGroup.alpha = _isActive ? _activeAlpha : _inactiveAlpha;
    }
}
