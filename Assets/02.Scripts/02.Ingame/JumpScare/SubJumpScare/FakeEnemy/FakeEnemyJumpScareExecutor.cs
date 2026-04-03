using UnityEngine;

public class FakeEnemyJumpScareExecutor : MonoBehaviour
{
    [Header("Placement - Sampling")]
    [SerializeField] private int _distanceSteps = 4;
    [SerializeField] private int _angleSteps = 7;

    [Header("Placement - World Filter")]
    [SerializeField] private LayerMask _environmentLayerMask;
    [SerializeField] private string _groundTag = "Ground";
    [SerializeField] private string _obstacleTag = "Obstacle";

    [Header("Placement - Ground Check")]
    [SerializeField] private float _groundProbeStartHeight = 1.5f;
    [SerializeField] private float _groundProbeDistance = 4.0f;

    [Header("Placement - Overlap Check")]
    [SerializeField] private float _bodyRadius = 0.35f;
    [SerializeField] private float _bodyHeight = 1.8f;
    [SerializeField] private int _overlapBufferSize = 16;

    [Header("Placement - Visibility Check")]
    [SerializeField] private float _chestHeight = 1.1f;

    [Header("Spawn")]
    [SerializeField] private Transform _spawnParent;
    [SerializeField] private float _defaultLifetime = 3.0f;

    [Header("Debug")]
    [SerializeField] private bool _enableDebugLog = false;

    private FakeEnemyPlacementResolver _placementResolver;
    private FakeEnemySpawnService _spawnService;

    public FakeEnemyPlacementDebugSnapshot DebugSnapshot
    {
        get
        {
            if (_placementResolver == null)
            {
                return null;
            }

            return _placementResolver.DebugSnapshot;
        }
    }

    public float ChestHeight
    {
        get
        {
            if (_placementResolver == null)
            {
                return _chestHeight;
            }

            return _placementResolver.ChestHeight;
        }
    }

    public float BodyRadius
    {
        get
        {
            if (_placementResolver == null)
            {
                return _bodyRadius;
            }

            return _placementResolver.BodyRadius;
        }
    }

    public float BodyHeight
    {
        get
        {
            if (_placementResolver == null)
            {
                return _bodyHeight;
            }

            return _placementResolver.BodyHeight;
        }
    }

    private void Awake()
    {
        RebuildServices();
    }

    private void OnValidate()
    {
        _distanceSteps = Mathf.Max(1, _distanceSteps);
        _angleSteps = Mathf.Max(1, _angleSteps);
        _overlapBufferSize = Mathf.Max(1, _overlapBufferSize);

        if (Application.isPlaying == true)
        {
            RebuildServices();
        }
    }

    public bool TryExecute(FakeEnemyJumpScareExecuteRequest request, out FakeEnemyInstance spawnedInstance)
    {
        spawnedInstance = null;

        if (request.IsValid() == false)
        {
            if (_enableDebugLog == true)
            {
                Debug.LogWarning("[FakeEnemyJumpScareExecutor] 실행 요청 데이터가 유효하지 않습니다.");
            }

            return false;
        }

        EnsureServices();

        FakeEnemyPlacementRequest placementRequest = CreatePlacementRequest(request);
        FakeEnemyPlacementResult placementResult;

        bool isPlaced = _placementResolver.TryResolve(placementRequest, out placementResult);

        if (isPlaced == false)
        {
            if (_enableDebugLog == true)
            {
                Debug.Log(string.Format(
                    "[FakeEnemyJumpScareExecutor] 배치 실패 | Reason={0}",
                    placementResult.FailReason));
            }

            return false;
        }

        bool isSpawned = _spawnService.TrySpawn(
            request,
            placementResult.Position,
            placementResult.Rotation,
            _defaultLifetime,
            out spawnedInstance);

        if (isSpawned == false)
        {
            if (_enableDebugLog == true)
            {
                Debug.LogWarning("[FakeEnemyJumpScareExecutor] 배치는 성공했지만 실제 생성에는 실패했습니다.");
            }

            return false;
        }

        return true;
    }

    public bool TryResolvePlacementForDebug(
        FakeEnemyJumpScareExecuteRequest request,
        out FakeEnemyPlacementResult placementResult)
    {
        placementResult = FakeEnemyPlacementResult.CreateFailure(EFakeEnemyPlacementFailReason.None);

        if (request.IsValid() == false)
        {
            return false;
        }

        EnsureServices();

        FakeEnemyPlacementRequest placementRequest = CreatePlacementRequest(request);
        return _placementResolver.TryResolve(placementRequest, out placementResult);
    }

    private void EnsureServices()
    {
        if (_placementResolver == null || _spawnService == null)
        {
            RebuildServices();
        }
    }

    private void RebuildServices()
    {
        _placementResolver = new FakeEnemyPlacementResolver(
            _distanceSteps,
            _angleSteps,
            _environmentLayerMask,
            _groundTag,
            _obstacleTag,
            _groundProbeStartHeight,
            _groundProbeDistance,
            _bodyRadius,
            _bodyHeight,
            _chestHeight,
            _overlapBufferSize,
            _enableDebugLog);

        _spawnService = new FakeEnemySpawnService(
            _spawnParent,
            _enableDebugLog);
    }

    private FakeEnemyPlacementRequest CreatePlacementRequest(FakeEnemyJumpScareExecuteRequest request)
    {
        return new FakeEnemyPlacementRequest(
            request.CameraPosition,
            request.CameraForward,
            request.PlayerTransform,
            request.MinDistance,
            request.MaxDistance,
            request.AllowedForwardAngle,
            request.UseDeterministicSelection,
            request.DeterministicSeed);
    }
}