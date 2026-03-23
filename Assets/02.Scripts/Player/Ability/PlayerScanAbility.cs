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
    }

    private void Update()
    {
        if (_owner.InteractMode != PlayerInteractMode.Scan)
        {
            return;
        }

        //소나
        if (_input.RmbPressInput)
        {
            _sonarScanFeature.TryScan();
        }
        if (!_sonarScanFeature.IsReady)
        {
            _sonarScanFeature.UpdateCoolDown();
        }
        
        //라이더
        if (_input.LmbPressInput)
        {
            _lidarScanFeature.UpdateScan(Time.deltaTime);
        }
        if (_input.InteractInput)
        {
            _lidarScanFeature.SubmitCurrentQte();
        }
        if (_input.LmbReleaseInput)
        {
            _lidarScanFeature.StopScan();
            
        }
    }
}
