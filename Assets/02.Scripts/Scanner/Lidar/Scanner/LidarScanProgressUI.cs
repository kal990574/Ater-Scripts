using UnityEngine;
using UnityEngine.UI;

public class LidarScanProgressUI : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private Slider _slider;
    [SerializeField] private LidarController _controller;

    private LidarTarget _currentTarget;

    private void Awake()
    {
        if (_controller == null)
        {
            _controller = FindFirstObjectByType<LidarController>();
        }

        if (_controller == null || _slider == null)
        {
            enabled = false;
            gameObject.SetActive(false);
            return;
        }

        _controller.OnTargetFind += SetTarget;
        _controller.OnTargetLost += ResetTarget;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_controller != null)
        {
            _controller.OnTargetFind -= SetTarget;
            _controller.OnTargetLost -= ResetTarget;
        }

        UnbindCurrentTarget();
    }

    public void SetTarget(LidarTarget target)
    {
        if (target == null)
        {
            ResetTarget();
            return;
        }

        if (_currentTarget == target)
        {
            Refresh(target.ProgressRatio);
            gameObject.SetActive(true);
            return;
        }

        UnbindCurrentTarget();
        _currentTarget = target;
        _currentTarget.OnProgressChanged += Refresh;
        Refresh(_currentTarget.ProgressRatio);
        gameObject.SetActive(true);
    }

    public void ResetTarget()
    {
        UnbindCurrentTarget();
        Refresh();
        gameObject.SetActive(false);
    }

    public void Refresh(float ratio = 0f)
    {
        _slider.value = ratio;
    }

    private void UnbindCurrentTarget()
    {
        if (_currentTarget == null)
        {
            return;
        }

        _currentTarget.OnProgressChanged -= Refresh;
        _currentTarget = null;
    }
}
