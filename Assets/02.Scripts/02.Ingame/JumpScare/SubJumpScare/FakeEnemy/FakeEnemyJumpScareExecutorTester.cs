using UnityEngine;

public class FakeEnemyJumpScareExecutorTester : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private FakeEnemyJumpScareExecutor _executor;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private FakeEnemySubJumpScareDefinitionSO _definition;

    [Header("Test Input")]
    [SerializeField] private KeyCode _testKey = KeyCode.T;
    [SerializeField] private bool _useDeterministicSelection = false;
    [SerializeField] private int _deterministicSeed = 12345;

    [Header("Debug")]
    [SerializeField] private bool _enableDebugLog = true;

    private void Update()
    {
        if (Input.GetKeyDown(_testKey) == false)
        {
            return;
        }

        if (HasRequiredReference() == false)
        {
            if (_enableDebugLog == true)
            {
                Debug.LogWarning("[FakeEnemyJumpScareExecutorTester] 필수 참조가 비어 있습니다.");
            }

            return;
        }

        FakeEnemyJumpScareExecuteRequest request = CreateRequest();

        if (request.IsValid() == false)
        {
            if (_enableDebugLog == true)
            {
                Debug.LogWarning("[FakeEnemyJumpScareExecutorTester] 정의 데이터가 유효하지 않습니다.");
            }

            return;
        }

        FakeEnemyInstance spawnedInstance;
        bool isSuccess = _executor.TryExecute(request, out spawnedInstance);

        if (_enableDebugLog == true)
        {
            Debug.Log(string.Format(
                "[FakeEnemyJumpScareExecutorTester] 테스트 실행 결과: {0}",
                isSuccess));
        }
    }

    private bool HasRequiredReference()
    {
        if (_executor == null)
        {
            return false;
        }

        if (_cameraTransform == null)
        {
            return false;
        }

        if (_playerTransform == null)
        {
            return false;
        }

        if (_definition == null)
        {
            return false;
        }

        return true;
    }

    private FakeEnemyJumpScareExecuteRequest CreateRequest()
    {
        return new FakeEnemyJumpScareExecuteRequest(
            _cameraTransform.position,
            _cameraTransform.forward,
            _playerTransform,
            _definition.MinSpawnDistance,
            _definition.MaxSpawnDistance,
            _definition.AllowedForwardAngle,
            _definition.PosePrefabs,
            _useDeterministicSelection,
            _deterministicSeed);
    }
}