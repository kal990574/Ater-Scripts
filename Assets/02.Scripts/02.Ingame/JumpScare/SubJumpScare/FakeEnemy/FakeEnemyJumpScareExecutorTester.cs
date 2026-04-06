using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteAlways]
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

    [Header("Gizmo Option")]
    [SerializeField] private bool _drawGizmos = true;
    [SerializeField, ShowIf(nameof(_drawGizmos))] private bool _drawWhenNotSelected = false;
    [SerializeField, ShowIf(nameof(_drawGizmos))] private float _pointRadius = 0.12f;
    [SerializeField, ShowIf(nameof(_drawGizmos))] private float _selectedPointRadius = 0.2f;
    [SerializeField, ShowIf(nameof(_drawGizmos))] private bool _drawSampleToGroundLine = true;
    [SerializeField, ShowIf(nameof(_drawGizmos))] private bool _drawCapsuleHint = true;
    [SerializeField, ShowIf(nameof(_drawGizmos))] private bool _drawFacingDirection = true;

    [Header("Candidate Colors - Ground Miss")]
    [SerializeField, ShowIf(nameof(_drawGizmos))] private Color _groundMissColor = Color.red;

    [Header("Candidate Colors - Invalid Ground Tag")]
    [SerializeField, ShowIf(nameof(_drawGizmos))] private Color _invalidGroundTagColor = new Color(0.6f, 0.2f, 1.0f, 1.0f);

    [Header("Candidate Colors - Invalid Distance")]
    [SerializeField, ShowIf(nameof(_drawGizmos))] private Color _invalidDistanceColor = Color.yellow;

    [Header("Candidate Colors - Overlap Blocked")]
    [SerializeField, ShowIf(nameof(_drawGizmos))] private Color _overlapBlockedColor = new Color(1.0f, 0.5f, 0.0f, 1.0f);

    [Header("Candidate Colors - Occluded")]
    [SerializeField, ShowIf(nameof(_drawGizmos))] private Color _occludedColor = Color.magenta;

    [Header("Candidate Colors - Valid")]
    [SerializeField, ShowIf(nameof(_drawGizmos))] private Color _validColor = Color.cyan;

    [Header("Candidate Colors - Selected")]
    [SerializeField, ShowIf(nameof(_drawGizmos))] private Color _selectedColor = Color.green;

    [Header("Candidate Colors - Default")]
    [SerializeField, ShowIf(nameof(_drawGizmos))] private Color _defaultColor = Color.white;

    [Header("Debug")]
    [SerializeField] private bool _enableDebugLog = true;

    private void Update()
    {
        if (Application.isPlaying == false)
        {
            return;
        }

        if (Input.GetKeyDown(_testKey) == false)
        {
            return;
        }

        FakeEnemyJumpScareExecuteRequest request;

        if (TryCreateRequest(out request) == false)
        {
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

    private void OnDrawGizmos()
    {
        if (_drawGizmos == false)
        {
            return;
        }
        
        if (_drawWhenNotSelected == false)
        {
            return;
        }

        DrawDebugGizmos();
    }

    private void OnDrawGizmosSelected()
    {
        if (_drawGizmos == false)
        {
            return;
        }
        
        DrawDebugGizmos();
    }

    private void DrawDebugGizmos()
    {
        FakeEnemyJumpScareExecuteRequest request;

        if (TryCreateRequest(out request) == false)
        {
            return;
        }

        FakeEnemyPlacementResult result;
        bool isResolved = _executor.TryResolvePlacementForDebug(request, out result);
        FakeEnemyPlacementDebugSnapshot snapshot = _executor.DebugSnapshot;

        if (snapshot != null)
        {
            DrawCandidates(snapshot);
        }

        if (isResolved == true && result.Success == true)
        {
            DrawSelectedResult(result);
            return;
        }

        if (_enableDebugLog == true)
        {
            Debug.Log(string.Format(
                "[FakeEnemyJumpScareExecutorTester] 배치 실패: {0}",
                result.FailReason));
        }
    }

    private bool TryCreateRequest(out FakeEnemyJumpScareExecuteRequest request)
    {
        request = default;

        if (HasRequiredReference() == false)
        {
            if (_enableDebugLog == true)
            {
                Debug.LogWarning("[FakeEnemyJumpScareExecutorTester] 필수 참조가 비어 있습니다.");
            }

            return false;
        }

        request = new FakeEnemyJumpScareExecuteRequest(
            _cameraTransform.position,
            _cameraTransform.forward,
            _playerTransform,
            _definition.MinSpawnDistance,
            _definition.MaxSpawnDistance,
            _definition.AllowedForwardAngle,
            _definition.PosePrefabs,
            _useDeterministicSelection,
            _deterministicSeed);

        if (request.IsValid() == false)
        {
            if (_enableDebugLog == true)
            {
                Debug.LogWarning("[FakeEnemyJumpScareExecutorTester] 정의 데이터가 유효하지 않습니다.");
            }

            return false;
        }

        return true;
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

    private void DrawCandidates(FakeEnemyPlacementDebugSnapshot snapshot)
    {
        for (int index = 0; index < snapshot.Candidates.Count; index++)
        {
            FakeEnemyPlacementCandidateDebugInfo candidate = snapshot.Candidates[index];
            DrawCandidate(candidate);
        }
    }

    private void DrawCandidate(FakeEnemyPlacementCandidateDebugInfo candidate)
    {
        Gizmos.color = GetCandidateColor(candidate.State);

        Vector3 drawPoint = GetCandidateDrawPoint(candidate);

        Gizmos.DrawSphere(drawPoint, _pointRadius);

        if (_drawSampleToGroundLine == true)
        {
            Gizmos.DrawLine(candidate.SamplePoint, drawPoint);
        }
    }

    private Vector3 GetCandidateDrawPoint(FakeEnemyPlacementCandidateDebugInfo candidate)
    {
        if (candidate.State == EFakeEnemyPlacementCandidateState.GroundMiss)
        {
            return candidate.SamplePoint;
        }

        return candidate.GroundedPoint;
    }

    private void DrawSelectedResult(FakeEnemyPlacementResult result)
    {
        Gizmos.color = _selectedColor;
        Gizmos.DrawSphere(result.Position, _selectedPointRadius);

        if (_drawCapsuleHint == true)
        {
            DrawCapsuleHint(result.Position, _executor.BodyRadius, _executor.BodyHeight);
        }

        if (_drawFacingDirection == true)
        {
            DrawFacingDirection(result);
        }
    }

    private void DrawFacingDirection(FakeEnemyPlacementResult result)
    {
        Vector3 forward = result.Rotation * Vector3.forward;
        Vector3 start = result.Position + (Vector3.up * _executor.ChestHeight);
        Vector3 end = start + (forward * 0.75f);

        Gizmos.DrawLine(start, end);
    }

    private Color GetCandidateColor(EFakeEnemyPlacementCandidateState state)
    {
        switch (state)
        {
            case EFakeEnemyPlacementCandidateState.GroundMiss:
                return _groundMissColor;

            case EFakeEnemyPlacementCandidateState.InvalidGroundTag:
                return _invalidGroundTagColor;

            case EFakeEnemyPlacementCandidateState.InvalidDistance:
                return _invalidDistanceColor;

            case EFakeEnemyPlacementCandidateState.OverlapBlocked:
                return _overlapBlockedColor;

            case EFakeEnemyPlacementCandidateState.Occluded:
                return _occludedColor;

            case EFakeEnemyPlacementCandidateState.Valid:
                return _validColor;

            case EFakeEnemyPlacementCandidateState.Selected:
                return _selectedColor;

            default:
                return _defaultColor;
        }
    }

    private void DrawCapsuleHint(Vector3 groundedPoint, float radius, float height)
    {
        Vector3 bottom = groundedPoint + (Vector3.up * radius);
        float topHeight = Mathf.Max(radius, height - radius);
        Vector3 top = groundedPoint + (Vector3.up * topHeight);

        Gizmos.DrawWireSphere(bottom, radius);
        Gizmos.DrawWireSphere(top, radius);

        Vector3 forwardOffset = Vector3.forward * radius;
        Vector3 rightOffset = Vector3.right * radius;

        Gizmos.DrawLine(bottom + forwardOffset, top + forwardOffset);
        Gizmos.DrawLine(bottom - forwardOffset, top - forwardOffset);
        Gizmos.DrawLine(bottom + rightOffset, top + rightOffset);
        Gizmos.DrawLine(bottom - rightOffset, top - rightOffset);
    }
}
