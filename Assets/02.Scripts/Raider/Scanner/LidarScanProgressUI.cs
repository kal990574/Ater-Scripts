using System;
using UnityEngine;
using UnityEngine.UI;

//스캔중일때 진행도를 보여주는 UI
//추후 호버일때는 80%의 투명도로 진행도 표현
public class LidarScanProgressUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    [SerializeField] private LidarController _controller;
    private LidarTarget _currentTarget;

    private void Awake()
    {
        if (_controller == null)
        {
            _controller = FindFirstObjectByType<LidarController>();
        }
        
        _controller.OnTargetFind += SetTarget;
        _controller.OnTargetLost += ResetTarget;
        
        gameObject.SetActive(false);
    }
    

    public void SetTarget(LidarTarget target)
    {
        _currentTarget = target;
        _currentTarget.OnProgressChanged += Refresh;
        gameObject.SetActive(true);
    }

    public void ResetTarget()
    {
        _currentTarget.OnProgressChanged -= Refresh;
        _currentTarget = null;
        gameObject.SetActive(false);
    }

    public void Refresh(float ratio = 0)
    {
        _slider.value = ratio;
    }
}
