using System;
using DG.Tweening;
using UnityEngine;

namespace _02.Scripts.Player
{
    public class PlayerModeTransitionAbility : PlayerAbility
    {
        [Header("References")]
        [SerializeField] private Transform _handRoot;
        [SerializeField] private PlayerHandAbility _handAbility;
        [SerializeField] private PlayerScanAbility _scanAbility;

        [Header("Tween")]
        [SerializeField] private Vector3 _loweredLocalPosition = new(0f, -0.2f, 0f);
        [SerializeField] private float _moveDuration = 0.18f;
        [SerializeField] private Ease _moveEase = Ease.OutQuad;

        private Sequence _transitionSequence;
        private Vector3 _defaultLocalPosition;
        private bool _isInitialized;

        public bool IsTransitioning => _transitionSequence != null && _transitionSequence.IsActive();

        private void Start()
        {
            if (_handAbility == null)
            {
                _handAbility = _owner != null ? _owner.GetAbility<PlayerHandAbility>() : null;
            }

            if (_scanAbility == null)
            {
                _scanAbility = _owner != null ? _owner.GetAbility<PlayerScanAbility>() : null;
            }

            if (_handRoot == null)
            {
                Debug.LogWarning($"{nameof(PlayerModeTransitionAbility)} requires a HandRoot reference.");
                return;
            }

            _defaultLocalPosition = _handRoot.localPosition;
            _isInitialized = _handRoot != null;
        }

        private void OnDisable()
        {
            KillSequence();

            if (_isInitialized)
            {
                _handRoot.localPosition = _defaultLocalPosition;
            }
        }

        public bool TryPlayTransition(Action onLowered, Action onCompleted = null)
        {
            if (CanStartTransition() == false)
            {
                return false;
            }

            _transitionSequence = DOTween.Sequence();
            _transitionSequence.Append(CreateMoveTween(_defaultLocalPosition + _loweredLocalPosition));
            _transitionSequence.AppendCallback(() => onLowered?.Invoke());
            _transitionSequence.Append(CreateMoveTween(_defaultLocalPosition));
            _transitionSequence.AppendCallback(() => onCompleted?.Invoke());
            _transitionSequence.OnKill(() => _transitionSequence = null);
            _transitionSequence.OnComplete(() => _transitionSequence = null);
            return true;
        }

        private bool CanStartTransition()
        {
            if (_isInitialized == false || _owner == null)
            {
                return false;
            }

            if (IsTransitioning)
            {
                return false;
            }

            return true;
        }

        private Tween CreateMoveTween(Vector3 targetLocalPosition)
        {
            return _handRoot.DOLocalMove(targetLocalPosition, _moveDuration).SetEase(_moveEase);
        }

        private void KillSequence()
        {
            if (_transitionSequence == null)
            {
                return;
            }

            if (_transitionSequence.IsActive())
            {
                _transitionSequence.Kill();
            }

            _transitionSequence = null;
        }
    }
}
