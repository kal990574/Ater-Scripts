using UnityEngine;

namespace _02.Scripts.Player
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Ater/Player/PlayerConfig")]
    public class PlayerConfigSO : ScriptableObject
    {
        [Header("Locomotion")]
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _moveSpeed = 3.5f;
        [SerializeField] [Range(0f, 1f)] private float _backwardSpeedMultiplier = 0.5f;
        [SerializeField] private float _mouseSensitivity = 0.15f;
        [SerializeField] private float _minPitch = -90f;
        [SerializeField] private float _maxPitch = 90f;

        public float Gravity => _gravity;
        public float MoveSpeed => _moveSpeed;
        public float BackwardSpeedMultiplier => _backwardSpeedMultiplier;
        public float MouseSensitivity => _mouseSensitivity;
        public float MinPitch => _minPitch;
        public float MaxPitch => _maxPitch;
    }
}
