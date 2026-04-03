using UnityEngine;

public class FakeEnemyJumpScareExecutor : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private FakeEnemyPlacementResolver _placementResolver;
    [SerializeField] private FakeEnemySpawnService _spawnService;

    [Header("Spawn Option")]
    [SerializeField] private float _defaultLifetime = 3.0f;

    [Header("Debug")]
    [SerializeField] private bool _enableDebugLog = false;

    public bool TryExecute(FakeEnemyJumpScareExecuteRequest request, out FakeEnemyInstance spawnedInstance)
    {
        spawnedInstance = null;

        if (HasRequiredReference() == false)
        {
            return false;
        }

        FakeEnemyPlacementRequest placementRequest = CreatePlacementRequest(request);

        FakeEnemyPlacementResult placementResult;
        bool isSuccess = _placementResolver.TryResolve(placementRequest, out placementResult);

        if (isSuccess == false)
        {
            if (_enableDebugLog == true)
            {
                Debug.Log(string.Format(
                    "[FakeEnemyJumpScareExecutor] 배치 실패. FailReason: {0}",
                    placementResult.FailReason));
            }

            return false;
        }

        bool isSpawned = _spawnService.TrySpawn(
            placementResult.Position,
            placementResult.Rotation,
            _defaultLifetime,
            out spawnedInstance);

        if (isSpawned == false)
        {
            if (_enableDebugLog == true)
            {
                Debug.LogWarning("[FakeEnemyJumpScareExecutor] 생성 서비스에서 가짜적 생성에 실패했습니다.");
            }

            return false;
        }

        if (_enableDebugLog == true)
        {
            Debug.Log(string.Format(
                "[FakeEnemyJumpScareExecutor] 실행 성공. Position: {0}",
                placementResult.Position));
        }

        return true;
    }

    private bool HasRequiredReference()
    {
        if (_placementResolver == null)
        {
            if (_enableDebugLog == true)
            {
                Debug.LogWarning("[FakeEnemyJumpScareExecutor] PlacementResolver 참조가 없습니다.");
            }

            return false;
        }

        if (_spawnService == null)
        {
            if (_enableDebugLog == true)
            {
                Debug.LogWarning("[FakeEnemyJumpScareExecutor] SpawnService 참조가 없습니다.");
            }

            return false;
        }

        return true;
    }

    private FakeEnemyPlacementRequest CreatePlacementRequest(FakeEnemyJumpScareExecuteRequest request)
    {
        return new FakeEnemyPlacementRequest(
            request.CameraPosition,
            request.CameraForward,
            request.PlayerPosition,
            request.MinDistance,
            request.MaxDistance,
            request.UseDeterministicSelection,
            request.DeterministicSeed);
    }
}