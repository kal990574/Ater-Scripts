using _02.Scripts.Sonar;
using UnityEngine;

namespace _02.Scripts.Player
{
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
            bool startVisible = _owner == null || _owner.InteractMode == EPlayerInteractMode.Scan;
            SetScannerVisible(startVisible);
        }

        private void Update()
        {
            _sonarScanFeature.UpdateCoolDown();
            _lidarScanFeature.UpdateEnergy(Time.deltaTime, _isLidarHolding);
        }

        public void SetScannerVisible(bool isVisible)
        {
            if (_scannerModel != null)
            {
                _scannerModel.SetActive(isVisible);
            }

            _isScannerActive = isVisible;
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
}
