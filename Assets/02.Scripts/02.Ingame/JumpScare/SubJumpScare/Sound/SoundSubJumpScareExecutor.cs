using System.Collections.Generic;
using UnityEngine;

public class SoundSubJumpScareExecutor : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool _enableDebugLog = false;

    [Header("Runtime")]
    [SerializeField] private bool _isPlaying;

    public bool IsPlaying
    {
        get
        {
            return _isPlaying;
        }
    }

    public SoundJumpScareExecutionResult TryExecute(SoundJumpScareExecutionRequest request)
    {
        if (request.IsValid() == false)
        {
            return SoundJumpScareExecutionResult.CreateFailure("사운드 실행 요청 데이터가 유효하지 않습니다.");
        }

        if (_isPlaying == true)
        {
            return SoundJumpScareExecutionResult.CreateFailure("이미 다른 서브 점프스케어가 실행 중입니다.");
        }

        if (TryResolve(request, out SoundJumpScareResolvedData resolvedData) == false)
        {
            return SoundJumpScareExecutionResult.CreateFailure("사운드 재생 데이터를 계산하지 못했습니다.");
        }

        _isPlaying = true;

        request.SoundService.PlaySFX(
            resolvedData.SoundKey,
            resolvedData.WorldPosition,
            resolvedData.Volume);

        _isPlaying = false;

        if (_enableDebugLog == true)
        {
            Debug.Log(string.Format(
                "[SoundSubJumpScareExecutor] Play | Key={0}, Position={1}, Volume={2}",
                resolvedData.SoundKey,
                resolvedData.WorldPosition,
                resolvedData.Volume));
        }

        return SoundJumpScareExecutionResult.CreateSuccess(resolvedData);
    }

    private bool TryResolve(
        SoundJumpScareExecutionRequest request,
        out SoundJumpScareResolvedData resolvedData)
    {
        resolvedData = default;

        SoundSubJumpScareDefinitionSO definition = request.Definition;
        string selectedKey = SelectRandomSoundKey(definition);

        if (string.IsNullOrWhiteSpace(selectedKey) == true)
        {
            return false;
        }

        Vector3 worldPosition = CalculateWorldPosition(
            request.PlayerRootTransform,
            request.PlayerCameraTransform,
            definition);

        resolvedData = new SoundJumpScareResolvedData(
            selectedKey,
            worldPosition,
            definition.Volume);

        return resolvedData.IsValid();
    }

    private string SelectRandomSoundKey(SoundSubJumpScareDefinitionSO definition)
    {
        if (definition == null || definition.SoundEntries == null || definition.SoundEntries.Count == 0)
        {
            return string.Empty;
        }

        List<SoundJumpScareClipEntry> validEntries = new List<SoundJumpScareClipEntry>();

        for (int index = 0; index < definition.SoundEntries.Count; index++)
        {
            if (definition.SoundEntries[index].IsValid() == true)
            {
                validEntries.Add(definition.SoundEntries[index]);
            }
        }

        if (validEntries.Count == 0)
        {
            return string.Empty;
        }

        int randomIndex = Random.Range(0, validEntries.Count);
        return validEntries[randomIndex].Key.Value;
    }

    private Vector3 CalculateWorldPosition(
        Transform playerRootTransform,
        Transform playerCameraTransform,
        SoundSubJumpScareDefinitionSO definition)
    {
        Vector3 origin = playerRootTransform.position;
        Vector3 direction = ResolveDirection(playerRootTransform, playerCameraTransform, definition.Direction);
        float distance = Random.Range(definition.MinDistance, definition.MaxDistance);
        float heightOffset = Random.Range(definition.HeightOffsetRange.x, definition.HeightOffsetRange.y);

        Vector3 position = origin + (direction * distance);
        position.y += heightOffset;

        return position;
    }

    private Vector3 ResolveDirection(
        Transform playerRootTransform,
        Transform playerCameraTransform,
        ESoundJumpScareDirection directionType)
    {
        Vector3 forward = playerCameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        if (forward.sqrMagnitude <= 0.0001f)
        {
            forward = playerRootTransform.forward;
            forward.y = 0f;
            forward.Normalize();
        }

        Vector3 right = playerRootTransform.right;
        right.y = 0f;
        right.Normalize();

        switch (directionType)
        {
            case ESoundJumpScareDirection.Forward:
            {
                return forward;
            }
            case ESoundJumpScareDirection.Backward:
            {
                return -forward;
            }
            case ESoundJumpScareDirection.Left:
            {
                return -right;
            }
            case ESoundJumpScareDirection.Right:
            {
                return right;
            }
            case ESoundJumpScareDirection.Around360:
            {
                float angle = Random.Range(0f, 360f);
                Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
                return rotation * Vector3.forward;
            }
            default:
            {
                return forward;
            }
        }
    }
}