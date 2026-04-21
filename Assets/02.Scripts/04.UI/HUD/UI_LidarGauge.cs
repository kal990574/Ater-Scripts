using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_LidarGauge : MonoBehaviour
{
    [SerializeField] private Image _filledImage;
    [Header("Color")]
    [SerializeField] private Color _emptyColor = Color.red;
    [SerializeField] private Color _fullColor = Color.white;
    [SerializeField] private float _colorTweenDuration = 0.2f;
    [Header("Binding")]
    [SerializeField] private LidarScanFeature _lidarScanFeature;

    private Tween _colorTween;

    //[Header("Test")]
    //[SerializeField] private float _dischargeRate = 0.3f;
    //[SerializeField] private float _chargeRate = 0.2f;
    //private float _testCharge = 1f;

    //private void Update()
    //{
    //    if (Input.GetMouseButton(1))
    //        _testCharge -= _dischargeRate * Time.deltaTime;
    //    else
    //        _testCharge += _chargeRate * Time.deltaTime;

    //    _testCharge = Mathf.Clamp01(_testCharge);
    //    SetFillAmount(_testCharge);
    //}
    private void Start()
    {
        if (_lidarScanFeature != null)
        {
            SetFillAmount(_lidarScanFeature.Energy);
            _lidarScanFeature.OnEnergyChanged += OnEnergyChanged;
        }
    }

    private void OnDestroy()
    {
        if (_lidarScanFeature != null)
        {
            _lidarScanFeature.OnEnergyChanged -= OnEnergyChanged;
        }

        _colorTween?.Kill();
    }

    private void OnEnergyChanged(float energy)
    {
        SetFillAmount(energy);
    }
    public void SetFillAmount(float amount)
    {
        if (_filledImage == null)
        {
            return;
        }
        
        _filledImage.fillAmount = amount;

        Color targetColor = Color.Lerp(_emptyColor, _fullColor, amount);
        _colorTween?.Kill();
        _colorTween = _filledImage
            .DOColor(targetColor, _colorTweenDuration)
            .SetEase(Ease.OutQuad);
    }
}
