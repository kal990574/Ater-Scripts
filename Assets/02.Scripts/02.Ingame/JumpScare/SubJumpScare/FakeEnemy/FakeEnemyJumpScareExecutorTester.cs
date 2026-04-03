using UnityEngine;

public class FakeEnemyJumpScareExecutorTester : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private FakeEnemyJumpScareExecutor _executor;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _playerTransform;

    [Header("Test Distance")]
    [SerializeField] private float _minDistance = 5.0f;
    [SerializeField] private float _maxDistance = 7.0f;

    [Header("Test Input")]
    [SerializeField] private KeyCode _testKey = KeyCode.T;
    [SerializeField] private bool _useDeterministicSelection = false;
    [SerializeField] private int _deterministicSeed = 12345;

    private void Update()
    {
        if (Input.GetKeyDown(_testKey) == false)
        {
            return;
        }

        if (_executor == null || _cameraTransform == null || _playerTransform == null)
        {
            return;
        }

        FakeEnemyJumpScareExecuteRequest request = new FakeEnemyJumpScareExecuteRequest(
            _cameraTransform.position,
            _cameraTransform.forward,
            _playerTransform.position,
            _minDistance,
            _maxDistance,
            _useDeterministicSelection,
            _deterministicSeed);

        FakeEnemyInstance spawnedInstance;
        _executor.TryExecute(request, out spawnedInstance);
    }
}