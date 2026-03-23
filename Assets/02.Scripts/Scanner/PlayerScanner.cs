using _02.Scripts.Player;
using _02.Scripts.Sonar;
using System;
using UnityEngine;

//소나와 라이더 스캔 관장
public class PlayerScanner : MonoBehaviour
{
    private IPlayerInput _input;
    
    [SerializeField] private LidarScanFeature _lidarScanFeature;
    [SerializeField] private SonarScanFeature _sonarScanFeature;

    private void Awake()
    {
        _input = GetComponentInParent<IPlayerInput>();
        if (_input == null)
        {
            Debug.LogError($"[{nameof(LidarScanFeature)}] {nameof(IPlayerInput)} not found.", this);
            enabled = false;
        }
        
        _lidarScanFeature.Initialize();
        _sonarScanFeature.Initialize();
    }

    private void Update()
    {
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
