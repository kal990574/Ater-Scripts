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
    [SerializeField] private bool _isActive = false;
    
    private void Start()
    {
        _lidarScanFeature.Initialize();
        _sonarScanFeature.Initialize();
        _isActive = true;
        _owner.OnModeChanged += SetScannerVisible;
    }

    private void SetScannerVisible(PlayerInteractMode mode)
    {
        if (mode == PlayerInteractMode.Scan)
        {
            _scannerModel.SetActive(true);
            _isActive = true;
        }
        else
        {
            _scannerModel.SetActive(false);
            _isActive = false;
        }
    }

    private void Update()
    {
        if (!_sonarScanFeature.IsReady)
        {
            _sonarScanFeature.UpdateCoolDown();
        }
    }

    private void OnDestroy()
    {
        _owner.OnModeChanged -= SetScannerVisible;
    }

    public void LidarScanDeactive()
    {
        _lidarScanFeature.StopScan();
    }

    public void LidarScanActiveAndUpdate()
    {
        if (!_isActive)
        {
            return;
        }
        _lidarScanFeature.UpdateScan(Time.deltaTime);
    }

    public void SonarActive()
    {
        if (!_isActive)
        {
            return;
        }
        _sonarScanFeature.TryScan();
    }
}
