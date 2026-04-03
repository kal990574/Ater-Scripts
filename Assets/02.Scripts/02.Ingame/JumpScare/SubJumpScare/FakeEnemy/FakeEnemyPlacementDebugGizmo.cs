using UnityEngine;

[ExecuteAlways]
public class FakeEnemyPlacementDebugGizmo : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private FakeEnemyPlacementResolver _resolver;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _playerTransform;

    [Header("Test Distance")]
    [SerializeField] private float _minDistance = 5.0f;
    [SerializeField] private float _maxDistance = 7.0f;

    [Header("Debug Selection")]
    [SerializeField] private bool _useDeterministicSelection = true;
    [SerializeField] private int _deterministicSeed = 12345;

    [Header("Gizmo Option")]
    [SerializeField] private bool _drawWhenNotSelected = false;
    [SerializeField] private float _pointRadius = 0.12f;
    [SerializeField] private float _selectedPointRadius = 0.2f;
    [SerializeField] private bool _drawSampleToGroundLine = true;
    [SerializeField] private bool _drawCapsuleHint = true;
    [SerializeField] private bool _drawFacingDirection = true;

    [Header("Candidate Colors - Ground Miss")]
    [SerializeField] private Color _groundMissColor = Color.red;

    [Header("Candidate Colors - Invalid Ground Tag")]
    [SerializeField] private Color _invalidGroundTagColor = new Color(0.6f, 0.2f, 1.0f, 1.0f);

    [Header("Candidate Colors - Invalid Distance")]
    [SerializeField] private Color _invalidDistanceColor = Color.yellow;

    [Header("Candidate Colors - Overlap Blocked")]
    [SerializeField] private Color _overlapBlockedColor = new Color(1.0f, 0.5f, 0.0f, 1.0f);

    [Header("Candidate Colors - Occluded")]
    [SerializeField] private Color _occludedColor = Color.magenta;

    [Header("Candidate Colors - Valid")]
    [SerializeField] private Color _validColor = Color.cyan;

    [Header("Candidate Colors - Selected")]
    [SerializeField] private Color _selectedColor = Color.green;

    [Header("Candidate Colors - Default")]
    [SerializeField] private Color _defaultColor = Color.white;

    private void OnDrawGizmos()
    {
        if (_drawWhenNotSelected == false)
        {
            return;
        }

        DrawDebug();
    }

    private void OnDrawGizmosSelected()
    {
        DrawDebug();
    }

    private void DrawDebug()
    {
        if (HasRequiredReference() == false)
        {
            return;
        }

        FakeEnemyPlacementRequest request = CreateRequest();
        FakeEnemyPlacementResult result;

        _resolver.TryResolve(request, out result);

        DrawCandidates(_resolver.DebugSnapshot);

        if (result.Success == true)
        {
            DrawSelectedResult(result);
        }
    }

    private bool HasRequiredReference()
    {
        if (_resolver == null)
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

        return true;
    }

    private FakeEnemyPlacementRequest CreateRequest()
    {
        return new FakeEnemyPlacementRequest(
            _cameraTransform.position,
            _cameraTransform.forward,
            _playerTransform.position,
            _minDistance,
            _maxDistance,
            _useDeterministicSelection,
            _deterministicSeed);
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
            DrawCapsuleHint(
                result.Position,
                _resolver.BodyRadius,
                _resolver.BodyHeight);
        }

        if (_drawFacingDirection == true)
        {
            DrawFacingDirection(result);
        }
    }

    private void DrawFacingDirection(FakeEnemyPlacementResult result)
    {
        Vector3 forward = result.Rotation * Vector3.forward;
        Vector3 start = result.Position + (Vector3.up * _resolver.ChestHeight);
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