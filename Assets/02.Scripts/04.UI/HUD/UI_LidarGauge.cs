using UnityEngine;
using UnityEngine.UI;

public class UI_LidarGauge : MonoBehaviour
{
    [SerializeField] private Image _filledImage;
    [Header("Binding")]
    [SerializeField] private LidarScanFeature _lidarScanFeature;

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
    }

    private void OnEnergyChanged(float energy)
    {
        SetFillAmount(energy);
    }
    public void SetFillAmount(float amount)
    {
        _filledImage.fillAmount = amount;
    }
}
