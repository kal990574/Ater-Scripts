using UnityEngine;
using UnityEngine.UI;

public class UI_LidarGuage : MonoBehaviour
{
    [SerializeField] private Image _filledImage;

    [Header("Test")]
    [SerializeField] private float _dischargeRate = 0.3f;
    [SerializeField] private float _chargeRate = 0.2f;
    private float _testCharge = 1f;

    private void Update()
    {
        if (Input.GetMouseButton(1))
            _testCharge -= _dischargeRate * Time.deltaTime;
        else
            _testCharge += _chargeRate * Time.deltaTime;

        _testCharge = Mathf.Clamp01(_testCharge);
        SetFillAmount(_testCharge);
    }

    public void SetFillAmount(float amount)
    {
        _filledImage.fillAmount = amount;
    }
}
