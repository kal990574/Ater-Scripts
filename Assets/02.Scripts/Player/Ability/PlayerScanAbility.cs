using _02.Scripts.Player;
using _02.Scripts.Sonar;
using System;
using UnityEngine;

//소나와 라이더 스캔 관장
public class PlayerScanAbility : PlayerAbility
{

    [SerializeField] private LidarScanFeature _lidarScanFeature;
    [SerializeField] private SonarScanFeature _sonarScanFeature;
    private IPlayerInput _input;
    private void Start()
    {
        _input = _owner.Input;
        _lidarScanFeature.Initialize();
        _sonarScanFeature.Initialize();
        _owner.OnModeChanged += SetScannerVisible;
    }

    private void SetScannerVisible(PlayerInteractMode mode)
    {
        if (mode == PlayerInteractMode.Scan)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
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

    public void LidarSubmitQTE()
    {
        _lidarScanFeature.SubmitCurrentQte();
    }

    public void LidarScanActiveAndUpdate()
    {
        _lidarScanFeature.UpdateScan(Time.deltaTime);
    }

    public void SonarActive()
    {
        _sonarScanFeature.TryScan();
    }
}
