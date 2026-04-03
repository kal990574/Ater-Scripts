using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class FakeEnemyPlacementResolver
{
    private readonly int _distanceSteps;
    private readonly int _angleSteps;

    private readonly LayerMask _environmentLayerMask;
    private readonly string _groundTag;
    private readonly string _obstacleTag;

    private readonly float _groundProbeStartHeight;
    private readonly float _groundProbeDistance;

    private readonly float _bodyRadius;
    private readonly float _bodyHeight;
    private readonly float _chestHeight;

    private readonly bool _enableDebugLog;

    private readonly FakeEnemyPlacementDebugSnapshot _debugSnapshot;
    private Collider[] _overlapResults;

    public FakeEnemyPlacementDebugSnapshot DebugSnapshot
    {
        get
        {
            return _debugSnapshot;
        }
    }

    public float ChestHeight
    {
        get
        {
            return _chestHeight;
        }
    }

    public float BodyRadius
    {
        get
        {
            return _bodyRadius;
        }
    }

    public float BodyHeight
    {
        get
        {
            return _bodyHeight;
        }
    }

    public FakeEnemyPlacementResolver(
        int distanceSteps,
        int angleSteps,
        LayerMask environmentLayerMask,
        string groundTag,
        string obstacleTag,
        float groundProbeStartHeight,
        float groundProbeDistance,
        float bodyRadius,
        float bodyHeight,
        float chestHeight,
        int overlapBufferSize,
        bool enableDebugLog)
    {
        _distanceSteps = Mathf.Max(1, distanceSteps);
        _angleSteps = Mathf.Max(1, angleSteps);

        _environmentLayerMask = environmentLayerMask;
        _groundTag = groundTag;
        _obstacleTag = obstacleTag;

        _groundProbeStartHeight = groundProbeStartHeight;
        _groundProbeDistance = groundProbeDistance;

        _bodyRadius = bodyRadius;
        _bodyHeight = bodyHeight;
        _chestHeight = chestHeight;

        _enableDebugLog = enableDebugLog;

        _debugSnapshot = new FakeEnemyPlacementDebugSnapshot();
        _overlapResults = new Collider[Mathf.Max(1, overlapBufferSize)];
    }

    public bool TryResolve(FakeEnemyPlacementRequest request, out FakeEnemyPlacementResult result)
    {
        _debugSnapshot.Clear();

        Vector3 flatForward = GetFlatForward(request.CameraForward);

        if (flatForward.sqrMagnitude <= 0.0001f)
        {
            result = FakeEnemyPlacementResult.CreateFailure(EFakeEnemyPlacementFailReason.InvalidForward);
            _debugSnapshot.SetResult(result);
            return false;
        }

        List<int> validCandidateIndices = new List<int>();
        List<FakeEnemyPlacementCandidateDebugInfo> candidates = _debugSnapshot.Candidates;

        for (int distanceIndex = 0; distanceIndex < _distanceSteps; distanceIndex++)
        {
            float distance = GetDistanceByStep(request, distanceIndex);

            for (int angleIndex = 0; angleIndex < _angleSteps; angleIndex++)
            {
                Vector3 samplePoint = CreateSamplePoint(
                    request.CameraPosition,
                    flatForward,
                    distance,
                    request.AllowedForwardAngle,
                    angleIndex);

                EvaluateCandidate(
                    request,
                    samplePoint,
                    candidates,
                    validCandidateIndices);
            }
        }

        if (validCandidateIndices.Count == 0)
        {
            result = FakeEnemyPlacementResult.CreateFailure(EFakeEnemyPlacementFailReason.NoValidCandidate);
            _debugSnapshot.SetResult(result);

            if (_enableDebugLog == true)
            {
                Debug.Log("[FakeEnemyPlacementResolver] 유효한 후보 위치를 찾지 못했습니다.");
            }

            return false;
        }

        int selectedCandidateIndex = SelectCandidateIndex(request, validCandidateIndices);
        MarkSelectedCandidate(candidates, selectedCandidateIndex);

        FakeEnemyPlacementCandidateDebugInfo selectedCandidate = candidates[selectedCandidateIndex];
        Quaternion rotation = CreateFacingRotation(selectedCandidate.GroundedPoint, request.PlayerTransform.position);

        result = FakeEnemyPlacementResult.CreateSuccess(
            selectedCandidate.GroundedPoint,
            rotation,
            selectedCandidateIndex);

        _debugSnapshot.SetResult(result);
        return true;
    }

    private Vector3 GetFlatForward(Vector3 cameraForward)
    {
        Vector3 flatForward = cameraForward;
        flatForward.y = 0.0f;

        if (flatForward.sqrMagnitude <= 0.0001f)
        {
            return Vector3.zero;
        }

        return flatForward.normalized;
    }

    private float GetDistanceByStep(FakeEnemyPlacementRequest request, int distanceIndex)
    {
        if (_distanceSteps == 1)
        {
            return request.MinDistance;
        }

        float normalizedStep = (float)distanceIndex / (_distanceSteps - 1);
        return Mathf.Lerp(request.MinDistance, request.MaxDistance, normalizedStep);
    }

    private Vector3 CreateSamplePoint(
        Vector3 cameraPosition,
        Vector3 flatForward,
        float distance,
        float allowedForwardAngle,
        int angleIndex)
    {
        float angleOffset = GetAngleOffset(allowedForwardAngle, angleIndex);
        Vector3 rotatedDirection = Quaternion.AngleAxis(angleOffset, Vector3.up) * flatForward;
        return cameraPosition + (rotatedDirection * distance);
    }

    private float GetAngleOffset(float allowedForwardAngle, int angleIndex)
    {
        if (_angleSteps == 1)
        {
            return 0.0f;
        }

        float normalizedStep = (float)angleIndex / (_angleSteps - 1);
        return Mathf.Lerp(-allowedForwardAngle, allowedForwardAngle, normalizedStep);
    }

    private void EvaluateCandidate(
        FakeEnemyPlacementRequest request,
        Vector3 samplePoint,
        List<FakeEnemyPlacementCandidateDebugInfo> candidates,
        List<int> validCandidateIndices)
    {
        Vector3 groundedPoint;
        EFakeEnemyPlacementCandidateState groundFailState;

        if (TryFindGroundPoint(samplePoint, out groundedPoint, out groundFailState) == false)
        {
            AddCandidate(candidates, samplePoint, samplePoint, groundFailState);
            return;
        }

        if (IsWithinDistanceRange(request, groundedPoint) == false)
        {
            AddCandidate(candidates, samplePoint, groundedPoint, EFakeEnemyPlacementCandidateState.InvalidDistance);
            return;
        }

        if (IsOverlappingObstacle(groundedPoint) == true)
        {
            AddCandidate(candidates, samplePoint, groundedPoint, EFakeEnemyPlacementCandidateState.OverlapBlocked);
            return;
        }

        if (IsOccludedByObstacle(request.CameraPosition, groundedPoint) == true)
        {
            AddCandidate(candidates, samplePoint, groundedPoint, EFakeEnemyPlacementCandidateState.Occluded);
            return;
        }

        AddCandidate(candidates, samplePoint, groundedPoint, EFakeEnemyPlacementCandidateState.Valid);
        validCandidateIndices.Add(candidates.Count - 1);
    }

    private void AddCandidate(
        List<FakeEnemyPlacementCandidateDebugInfo> candidates,
        Vector3 samplePoint,
        Vector3 groundedPoint,
        EFakeEnemyPlacementCandidateState state)
    {
        candidates.Add(new FakeEnemyPlacementCandidateDebugInfo(samplePoint, groundedPoint, state));
    }

    private bool TryFindGroundPoint(
        Vector3 samplePoint,
        out Vector3 groundedPoint,
        out EFakeEnemyPlacementCandidateState failState)
    {
        Vector3 rayStart = samplePoint + (Vector3.up * _groundProbeStartHeight);

        RaycastHit hit;
        bool isHit = Physics.Raycast(
            rayStart,
            Vector3.down,
            out hit,
            _groundProbeDistance,
            _environmentLayerMask,
            QueryTriggerInteraction.Ignore);

        if (isHit == false)
        {
            groundedPoint = samplePoint;
            failState = EFakeEnemyPlacementCandidateState.GroundMiss;
            return false;
        }

        if (IsGroundCollider(hit.collider) == false)
        {
            groundedPoint = hit.point;
            failState = EFakeEnemyPlacementCandidateState.InvalidGroundTag;
            return false;
        }

        groundedPoint = hit.point;
        failState = EFakeEnemyPlacementCandidateState.None;
        return true;
    }

    private bool IsGroundCollider(Collider targetCollider)
    {
        if (targetCollider == null)
        {
            return false;
        }

        return targetCollider.CompareTag(_groundTag);
    }

    private bool IsObstacleCollider(Collider targetCollider)
    {
        if (targetCollider == null)
        {
            return false;
        }

        return targetCollider.CompareTag(_obstacleTag);
    }

    private bool IsWithinDistanceRange(FakeEnemyPlacementRequest request, Vector3 groundedPoint)
    {
        Vector3 offset = groundedPoint - request.PlayerTransform.position;
        offset.y = 0.0f;

        float flatDistance = offset.magnitude;

        if (flatDistance < request.MinDistance)
        {
            return false;
        }

        if (flatDistance > request.MaxDistance)
        {
            return false;
        }

        return true;
    }

    private bool IsOverlappingObstacle(Vector3 groundedPoint)
    {
        Vector3 bottom = groundedPoint + (Vector3.up * _bodyRadius);
        float topHeight = Mathf.Max(_bodyRadius, _bodyHeight - _bodyRadius);
        Vector3 top = groundedPoint + (Vector3.up * topHeight);

        int hitCount = Physics.OverlapCapsuleNonAlloc(
            bottom,
            top,
            _bodyRadius,
            _overlapResults,
            _environmentLayerMask,
            QueryTriggerInteraction.Ignore);

        for (int index = 0; index < hitCount; index++)
        {
            Collider targetCollider = _overlapResults[index];

            if (IsObstacleCollider(targetCollider) == true)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsOccludedByObstacle(Vector3 cameraPosition, Vector3 groundedPoint)
    {
        Vector3 targetPoint = groundedPoint + (Vector3.up * _chestHeight);
        Vector3 directionToTarget = targetPoint - cameraPosition;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget <= 0.0001f)
        {
            return true;
        }

        Vector3 rayDirection = directionToTarget / distanceToTarget;

        RaycastHit hit;
        bool isHit = Physics.Raycast(
            cameraPosition,
            rayDirection,
            out hit,
            distanceToTarget,
            _environmentLayerMask,
            QueryTriggerInteraction.Ignore);

        if (isHit == false)
        {
            return false;
        }

        return IsObstacleCollider(hit.collider);
    }

    private int SelectCandidateIndex(FakeEnemyPlacementRequest request, List<int> validCandidateIndices)
    {
        int selectedValidIndex = GetRandomValidIndex(request, validCandidateIndices.Count);
        return validCandidateIndices[selectedValidIndex];
    }

    private int GetRandomValidIndex(FakeEnemyPlacementRequest request, int validCandidateCount)
    {
        if (request.UseDeterministicSelection == true)
        {
            System.Random random = new System.Random(request.DeterministicSeed);
            return random.Next(0, validCandidateCount);
        }

        return UnityEngine.Random.Range(0, validCandidateCount);
    }

    private void MarkSelectedCandidate(
        List<FakeEnemyPlacementCandidateDebugInfo> candidates,
        int selectedCandidateIndex)
    {
        FakeEnemyPlacementCandidateDebugInfo selectedCandidate = candidates[selectedCandidateIndex];
        selectedCandidate.State = EFakeEnemyPlacementCandidateState.Selected;
        candidates[selectedCandidateIndex] = selectedCandidate;
    }

    private Quaternion CreateFacingRotation(Vector3 spawnPosition, Vector3 playerPosition)
    {
        Vector3 forward = playerPosition - spawnPosition;
        forward.y = 0.0f;

        if (forward.sqrMagnitude <= 0.0001f)
        {
            return Quaternion.identity;
        }

        return Quaternion.LookRotation(forward.normalized, Vector3.up);
    }
}