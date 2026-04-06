using _02.Scripts.Player;
using _02.Scripts.Sonar;
using System;
using UnityEngine;

//소나와 라이더 스캔 관장
public class PlayerScanAbility : PlayerAbility
{
    [SerializeField] private LidarScanFeature _lidarScanFeature;
    [SerializeField] private SonarScanFeature _sonarScanFeature;

    [SerializeField] private GameObject _scannerModel;
    [SerializeField] private bool _isScannerActive = false;

    private bool _isLidarHolding = false;
    
    private void Start()
    {
        _lidarScanFeature.Initialize();
        _sonarScanFeature.Initialize();
        _isScannerActive = true;
        _owner.OnModeChanged += SetScannerVisible;
    }

    private void SetScannerVisible(EPlayerInteractMode mode)
    {
        if (mode == EPlayerInteractMode.Scan)
        {
            _scannerModel.SetActive(true);
            _isScannerActive = true;
        }
        else
        {
            _scannerModel.SetActive(false);
            _isScannerActive = false;
        }
    }

    private void Update()
    {
        _sonarScanFeature.UpdateCoolDown();
        _lidarScanFeature.UpdateEnergy(Time.deltaTime, _isLidarHolding);
    }

    private void OnDestroy()
    {
        _owner.OnModeChanged -= SetScannerVisible;
    }

    public void LidarScanDeactive()
    {
        _isLidarHolding = false;
        _lidarScanFeature.StopScan();
    }

    public void LidarScanUpdate()
    {
        if (!_isScannerActive)
        {
            return;
        }
        _lidarScanFeature.UpdateScan(Time.deltaTime);
    }

    public void LidarScanActive()
    {
        if (!_isScannerActive)
        {
            return;
        }
        _isLidarHolding = true;
        _lidarScanFeature.ActiveScan();
    }

    public void SonarActive()
    {
        if (!_isScannerActive)
        {
            return;
        }
        _sonarScanFeature.TryScan();
    }
}
