using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundSFXEmitter : MonoBehaviour
{
    private enum EmitPointType
    {
        Self,
        TargetTransform,
        WorldPosition,
        LocalPositionOnSelf
    }

    [Header("Sound")]
    [SerializeField] private SoundKeyReference _soundKey;
    [SerializeField] private float _volume = 1f;

    [Header("Emit Point")]
    [SerializeField] private EmitPointType _emitPointType = EmitPointType.Self;
    [SerializeField, ShowIf(nameof(UsesTargetTransform))] private Transform _targetTransform;
    [SerializeField, ShowIf(nameof(UsesWorldPosition))] private Vector3 _worldPosition;
    [SerializeField, ShowIf(nameof(UsesLocalPositionOnSelf))] private Vector3 _localPositionOnSelf;

    private ISoundService SoundService => SoundManager.Instance;

    private bool UsesTargetTransform => _emitPointType == EmitPointType.TargetTransform;
    private bool UsesWorldPosition => _emitPointType == EmitPointType.WorldPosition;
    private bool UsesLocalPositionOnSelf => _emitPointType == EmitPointType.LocalPositionOnSelf;

    [Button]
    public void Play()
    {
        if (TryValidateSoundKey() == false) return;

        ISoundService soundService = SoundService;
        if (soundService == null)
        {
            Debug.LogWarning($"[{nameof(SoundSFXEmitter)}] Sound service is not available.", this);
            return;
        }

        soundService.PlaySFX(_soundKey, ResolvePosition(), _volume);
    }

    public void PlayAtSelf()
    {
        PlayAt(transform.position);
    }

    public void PlayAtTarget(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning($"[{nameof(SoundSFXEmitter)}] Target transform is null.", this);
            return;
        }

        PlayAt(target.position);
    }

    public void PlayAt(Vector3 position)
    {
        if (TryValidateSoundKey() == false) return;

        ISoundService soundService = SoundService;
        if (soundService == null)
        {
            Debug.LogWarning($"[{nameof(SoundSFXEmitter)}] Sound service is not available.", this);
            return;
        }

        soundService.PlaySFX(_soundKey, position, _volume);
    }

    public void SetTargetTransform(Transform targetTransform)
    {
        _targetTransform = targetTransform;
    }

    public void SetWorldPosition(Vector3 worldPosition)
    {
        _worldPosition = worldPosition;
    }

    public void SetLocalPositionOnSelf(Vector3 localPositionOnSelf)
    {
        _localPositionOnSelf = localPositionOnSelf;
    }

    private Vector3 ResolvePosition()
    {
        switch (_emitPointType)
        {
            case EmitPointType.TargetTransform:
                return _targetTransform != null ? _targetTransform.position : transform.position;

            case EmitPointType.WorldPosition:
                return _worldPosition;

            case EmitPointType.LocalPositionOnSelf:
                return transform.TransformPoint(_localPositionOnSelf);

            default:
                return transform.position;
        }
    }

    private bool TryValidateSoundKey()
    {
        if (_soundKey.IsEmpty)
        {
            Debug.LogWarning($"[{nameof(SoundSFXEmitter)}] Sound key is empty. {BuildDebugContext()}", this);
            return false;
        }

        if (_soundKey.IsValid == false)
        {
            Debug.LogWarning($"[{nameof(SoundSFXEmitter)}] Broken sound key '{_soundKey}'. {BuildDebugContext()}", this);
            return false;
        }

        return true;
    }

    private string BuildDebugContext()
    {
        Vector3 position = ResolvePosition();
        string sceneName = gameObject.scene.IsValid() ? gameObject.scene.name : SceneManager.GetActiveScene().name;
        return $"Scene='{sceneName}', ObjectPath='{GetHierarchyPath(transform)}', Position={position}";
    }

    private static string GetHierarchyPath(Transform current)
    {
        if (current == null)
        {
            return "<null>";
        }

        string path = current.name;
        Transform parent = current.parent;

        while (parent != null)
        {
            path = $"{parent.name}/{path}";
            parent = parent.parent;
        }

        return path;
    }
}
