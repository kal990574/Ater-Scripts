using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessSubJumpScareExecutor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Volume _targetVolume;

    [Header("Services")]
    [SerializeField] private DarknessPostProcessService _darknessService = new DarknessPostProcessService();
    [SerializeField] private GrayscalePostProcessService _grayscaleService = new GrayscalePostProcessService();
    [SerializeField] private NoisePostProcessService _noiseService = new NoisePostProcessService();

    [Header("Debug")]
    [SerializeField,ReadOnly] private bool _isInitialized;
    [SerializeField,ReadOnly] private bool _isPlaying;
    [SerializeField,ReadOnly] private PostProcessSubJumpScareDefinitionSO _currentDefinition;
    [SerializeField,ReadOnly] private Coroutine _playCoroutine;

    private VolumeProfile _runtimeProfile;

    private Vignette _vignette;
    private ColorAdjustments _colorAdjustments;
    private FilmGrain _filmGrain;
    private ChromaticAberration _chromaticAberration;
    private LensDistortion _lensDistortion;

    private ISoundService _sound => SoundManager.Instance;
    public bool IsInitialized
    {
        get
        {
            return _isInitialized;
        }
    }

    public bool IsPlaying
    {
        get
        {
            return _isPlaying;
        }
    }

    private void Awake()
    {
        Initialize();
    }

    private void OnDisable()
    {
        StopCurrentEffect();
    }

    private void OnDestroy()
    {
        StopCurrentEffect();
    }

    public bool Initialize()
    {
        if (_isInitialized == true)
        {
            return true;
        }

        if (_targetVolume == null)
        {
            Debug.LogError("[PostProcessSubJumpScareExecutor] Target Volume이 비어 있습니다.", this);
            return false;
        }

        _runtimeProfile = _targetVolume.profile;

        if (_runtimeProfile == null)
        {
            Debug.LogError("[PostProcessSubJumpScareExecutor] Runtime Profile을 가져오지 못했습니다.", this);
            return false;
        }

        bool isSuccess = true;

        isSuccess &= TryGetOrAdd(out _vignette);
        isSuccess &= TryGetOrAdd(out _colorAdjustments);
        isSuccess &= TryGetOrAdd(out _filmGrain);
        isSuccess &= TryGetOrAdd(out _chromaticAberration);
        isSuccess &= TryGetOrAdd(out _lensDistortion);

        if (isSuccess == false)
        {
            Debug.LogError("[PostProcessSubJumpScareExecutor] 필요한 VolumeComponent 초기화에 실패했습니다.", this);
            return false;
        }

        PostProcessRuntimeContext context = new PostProcessRuntimeContext(
            _vignette,
            _colorAdjustments,
            _filmGrain,
            _chromaticAberration,
            _lensDistortion
        );

        _darknessService.Initialize(context);
        _grayscaleService.Initialize(context);
        _noiseService.Initialize(context);

        _isInitialized = true;
        return true;
    }

    private bool TryGetOrAdd<T>(out T component) where T : VolumeComponent
    {
        if (_runtimeProfile.TryGet(out component) == true)
        {
            return true;
        }

        component = _runtimeProfile.Add<T>(true);
        return component != null;
    }

    public bool TryExecute(PostProcessSubJumpScareDefinitionSO definition)
    {
        if (definition == null)
        {
            Debug.LogWarning("[PostProcessSubJumpScareExecutor] Definition이 null입니다.", this);
            return false;
        }

        if (Initialize() == false)
        {
            return false;
        }

        if (_isPlaying == true)
        {
            Debug.Log("[PostProcessSubJumpScareExecutor] 이미 실행 중이므로 새 요청을 무시합니다.", this);
            return false;
        }

        _playCoroutine = StartCoroutine(CoPlay(definition));
        return true;
    }

    private IEnumerator CoPlay(PostProcessSubJumpScareDefinitionSO definition)
    {
        _isPlaying = true;
        _currentDefinition = definition;

        if (_sound != null && definition.Sound.Value != null)
        {
            _sound.PlaySFX2D(definition.Sound, definition.SoundVolume);
        }
        
        bool isApplied = ApplyEffect(definition);
        if (isApplied == false)
        {
            _isPlaying = false;
            _currentDefinition = null;
            _playCoroutine = null;
            yield break;
        }

        float duration = Mathf.Max(0.0f, definition.Duration);

        if (duration > 0.0f)
        {
            yield return new WaitForSeconds(duration);
        }

        RestoreEffect(definition.EffectType);

        _isPlaying = false;
        _currentDefinition = null;
        _playCoroutine = null;
    }

    private bool ApplyEffect(PostProcessSubJumpScareDefinitionSO definition)
    {
        if (definition == null)
        {
            Debug.LogWarning("[PostProcessSubJumpScareExecutor] ApplyEffect 실패: Definition이 null입니다.", this);
            return false;
        }

        switch (definition.EffectType)
        {
            case EPostProcessEffectType.Darkness:
            {
                _darknessService.CaptureSnapshot();
                _darknessService.Apply(definition.EffectStrength);
                return true;
            }
            case EPostProcessEffectType.Grayscale:
            {
                _grayscaleService.CaptureSnapshot();
                _grayscaleService.Apply(definition.EffectStrength);
                return true;
            }
            case EPostProcessEffectType.Noise:
            {
                _noiseService.CaptureSnapshot();
                _noiseService.Apply(definition.Common.Intensity, definition.EffectStrength);
                return true;
            }
            default:
            {
                Debug.LogWarning("[PostProcessSubJumpScareExecutor] 지원하지 않는 EffectType입니다.", this);
                return false;
            }
        }
    }

    public void RestoreEffect(EPostProcessEffectType effectType)
    {
        if (_isInitialized == false)
        {
            return;
        }

        switch (effectType)
        {
            case EPostProcessEffectType.Darkness:
            {
                _darknessService.Restore();
                break;
            }
            case EPostProcessEffectType.Grayscale:
            {
                _grayscaleService.Restore();
                break;
            }
            case EPostProcessEffectType.Noise:
            {
                _noiseService.Restore();
                break;
            }
        }
    }

    [Button("현재 효과 중지")]
    public void StopCurrentEffect()
    {
        if (_playCoroutine != null)
        {
            StopCoroutine(_playCoroutine);
            _playCoroutine = null;
        }

        if (_currentDefinition != null)
        {
            RestoreEffect(_currentDefinition.EffectType);
        }

        _isPlaying = false;
        _currentDefinition = null;
    }

    [Button("모든 효과 복구")]
    public void RestoreAll()
    {
        if (_isInitialized == false)
        {
            return;
        }

        _darknessService.Restore();
        _grayscaleService.Restore();
        _noiseService.Restore();
    }
}