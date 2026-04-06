using Sirenix.OdinInspector;
using UnityEngine;

public class DropJumpScare : MainJumpScareBase
{
    [Header("References")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private SoundSFXEmitter _impactSound;

    [Header("Collision Filter")]
    [SerializeField] private LayerMask _environmentLayerMask;
    [SerializeField] private string _groundTag = "Ground";

    [Header("Stop Detection")]
    [SerializeField] private float _stopVelocityThreshold = 0.05f;
    [SerializeField] private float _stopAngularVelocityThreshold = 0.05f;
    [SerializeField] private float _forceFinishDelayAfterImpact = 1.5f;

    [Header("Debug")]
    [SerializeField] private bool _enableLog = true;

    [Header("Runtime")]
    [SerializeField, ReadOnly] private bool _isDropping;
    [SerializeField, ReadOnly] private bool _hasValidImpact;
    [SerializeField, ReadOnly] private bool _isFinishing;
    [SerializeField, ReadOnly] private float _impactElapsedTime;

    [Header("Cached Initial State")]
    [SerializeField, ReadOnly] private Vector3 _initialWorldPosition;
    [SerializeField, ReadOnly] private Quaternion _initialWorldRotation;
    [SerializeField, ReadOnly] private bool _initialIsKinematic;

    private bool _isInitialStateCached;

    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody>();
        CacheInitialState();
        InitializePhysicsState();
    }

    private void Reset()
    {
        
    }

    private void Update()
    {
        if (_hasValidImpact == false)
        {
            return;
        }

        if (_isFinishing == true)
        {
            return;
        }

        _impactElapsedTime += Time.deltaTime;

        if (IsStopped() == true)
        {
            FinishJumpScare();
            return;
        }

        if (_impactElapsedTime >= _forceFinishDelayAfterImpact)
        {
            if (_enableLog == true)
            {
                Debug.Log($"[{name}] 강제 종료 지연시간에 도달하여 점프스케어를 종료합니다.", this);
            }

            FinishJumpScare();
        }
    }

    [Button]
    protected override void OnExecute()
    {
        if (_rigidbody == null)
        {
            Debug.LogError($"[{name}] Rigidbody 가 없어  점프스케어를 실행할 수 없습니다.", this);
            NotifyFinished();
            return;
        }

        _isDropping = true;
        _hasValidImpact = false;
        _isFinishing = false;
        _impactElapsedTime = 0.0f;

        _rigidbody.isKinematic = false;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.WakeUp();

        if (_enableLog == true)
        {
            Debug.Log($"[{name}]  낙하를 시작합니다.", this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isDropping == false)
        {
            return;
        }

        if (_hasValidImpact == true)
        {
            return;
        }

        if (IsValidGroundCollision(collision) == false)
        {
            return;
        }

        _hasValidImpact = true;
        _impactElapsedTime = 0.0f;

        if (_enableLog == true)
        {
            Debug.Log($"[{name}] 첫 유효 충돌을 감지했습니다. 대상: {collision.gameObject.name}", this);
        }

        PlayImpactSound();
    }

    private bool IsValidGroundCollision(Collision collision)
    {
        if (collision == null)
        {
            return false;
        }

        GameObject targetObject = collision.gameObject;
        if (targetObject == null)
        {
            return false;
        }

        if (IsInLayerMask(targetObject.layer, _environmentLayerMask) == false)
        {
            return false;
        }

        if (targetObject.CompareTag(_groundTag) == false)
        {
            return false;
        }

        return true;
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        int layerBit = 1 << layer;
        return (layerMask.value & layerBit) != 0;
    }

    private void PlayImpactSound()
    {
        if (_impactSound == null)
        {
            if (_enableLog == true)
            {
                Debug.LogWarning($"[{name}] _impactSound 가 없어 충돌 사운드를 재생하지 않습니다.", this);
            }

            return;
        }

        _impactSound.Play();
    }

    private bool IsStopped()
    {
        if (_rigidbody == null)
        {
            return true;
        }

        float linearSpeed = _rigidbody.linearVelocity.magnitude;
        float angularSpeed = _rigidbody.angularVelocity.magnitude;

        bool isLinearStopped = linearSpeed <= _stopVelocityThreshold;
        bool isAngularStopped = angularSpeed <= _stopAngularVelocityThreshold;

        return isLinearStopped && isAngularStopped;
    }

    private void FinishJumpScare()
    {
        if (_isFinishing == true)
        {
            return;
        }

        _isFinishing = true;
        _isDropping = false;

        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;
        }

        if (_enableLog == true)
        {
            Debug.Log($"[{name}] 점프스케어를 종료합니다.", this);
        }

        NotifyFinished();
    }

    [Button]
    public void ResetJumpScare()
    {
        if (_isInitialStateCached == false)
        {
            CacheInitialState();
        }

        _isDropping = false;
        _hasValidImpact = false;
        _isFinishing = false;
        _impactElapsedTime = 0.0f;

        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            _rigidbody.Sleep();
        }

        transform.position = _initialWorldPosition;
        transform.rotation = _initialWorldRotation;

        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = _initialIsKinematic;
        }

        if (_enableLog == true)
        {
            Debug.Log($"[{name}] 점프스케어를 초기 상태로 리셋했습니다.", this);
        }
    }

    private void CacheInitialState()
    {
        _initialWorldPosition = transform.position;
        _initialWorldRotation = transform.rotation;
        _initialIsKinematic = _rigidbody != null && _rigidbody.isKinematic;
        _isInitialStateCached = true;
    }

    private void InitializePhysicsState()
    {
        _isDropping = false;
        _hasValidImpact = false;
        _isFinishing = false;
        _impactElapsedTime = 0.0f;

        if (_rigidbody == null)
        {
            return;
        }

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.isKinematic = true;
    }
}