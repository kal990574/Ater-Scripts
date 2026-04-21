using UnityEngine;

namespace _02.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerFootstepAbility : PlayerAbility
    {
        private CharacterController _characterController;
        private IPlayerInput _playerInput;
        private ISoundService _soundService;

        private AudioSource _footstepSource;
        private bool _isChasing;

        public void SetChasing(bool chasing)
        {
            if (_isChasing == chasing) return;
            _isChasing = chasing;

            if (_footstepSource != null)
            {
                _soundService.StopLoopSFX(_footstepSource);
                _footstepSource = null;
            }
        }

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _playerInput = GetComponent<IPlayerInput>();
            _soundService = SoundManager.Instance;
        }

        private void Update()
        {
            bool shouldPlay = _owner.CanMove && _playerInput.MoveInput.magnitude > 0.1f && _characterController.isGrounded;

            if (shouldPlay && _footstepSource == null)
            {
                // 루프 재생 시작
                string key = _isChasing ? SoundKey.Player_Running : SoundKey.Player_Walking;
                _footstepSource = _soundService.PlayLoopSFX(key, transform.position);
            }
            else if (!shouldPlay && _footstepSource != null)
            {
                // 멈추면 즉시 정지 및 풀 반환
                _soundService.StopLoopSFX(_footstepSource);
                _footstepSource = null;
            }

            // 재생 중인 사운드, 플레이어 위치 동기화
            if (_footstepSource != null)
            {
                _footstepSource.transform.position = transform.position;
            }
        }

        private void OnDestroy()
        {
            if (_footstepSource != null)
            {
                _soundService.StopLoopSFX(_footstepSource);
                _footstepSource = null;
            }
        }
    }
}