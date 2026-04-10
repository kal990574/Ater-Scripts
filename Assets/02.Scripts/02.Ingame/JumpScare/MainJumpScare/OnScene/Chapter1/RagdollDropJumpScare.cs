using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class RagdollDropJumpScare : MainJumpScareBase
{
    [Serializable]
    private struct RagdollBoneState
    {
        public Transform Transform;
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
    }

    [Serializable]
    private struct RagdollRigidbodyState
    {
        public Rigidbody Rigidbody;
        public bool IsKinematic;
    }
    
    [SerializeField] private GameObject _targetObject;
    
    [Header("Collision Filter")]
    [SerializeField] private LayerMask _environmentLayerMask;
    [SerializeField] private string _groundTag = "Ground";

    [Header("Stop Detection")]
    [SerializeField] private float _stopVelocityThreshold = 0.05f;
    [SerializeField] private float _stopAngularVelocityThreshold = 0.05f;
    [SerializeField] private float _forceFinishDelayAfterImpact = 1.5f;

    [Header("Reset Options")]
    [SerializeField] private bool _disableAnimatorWhileDropping = true;

    [Header("Debug")]
    [SerializeField] private bool _enableLog = true;

    [Header("Runtime")]
    [SerializeField, ReadOnly] private bool _isDropping;
    [SerializeField, ReadOnly] private bool _hasValidImpact;
    [SerializeField, ReadOnly] private bool _isFinishing;
    [SerializeField, ReadOnly] private float _impactElapsedTime;

    [Header("Collected State")]
    [SerializeField, ReadOnly] private List<Rigidbody> _ragdollRigidbodies = new List<Rigidbody>();
    [SerializeField, ReadOnly] private List<Transform> _ragdollBones = new List<Transform>();

    [Header("Cached Initial State")]
    [SerializeField, ReadOnly] private Vector3 _initialRootWorldPosition;
    [SerializeField, ReadOnly] private Quaternion _initialRootWorldRotation;
    [SerializeField, ReadOnly] private bool _initialAnimatorEnabled;

    [SerializeField] private UnityEvent _onDropCollision;

    private readonly List<RagdollBoneState> _cachedBoneStates = new List<RagdollBoneState>();
    private readonly List<RagdollRigidbodyState> _cachedRigidbodyStates = new List<RagdollRigidbodyState>();
    private bool _isInitialStateCached;

    protected GameObject TargetObject => _targetObject != null ? _targetObject : gameObject;
    
    protected override void Awake()
    {
        base.Awake();
        InitializeReferences();
        CacheInitialState();
        InitializePhysicsState();
    }

    private void Reset()
    {
        CollectRagdollParts();
    }

    private void Update()
    {
        if (_hasValidImpact == false || _isFinishing == true)
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
                Debug.Log($"[{name}] 랙돌 강제 종료 지연시간에 도달하여 점프스케어를 종료합니다.", this);
            }

            FinishJumpScare();
        }
    }

    protected override void OnExecute()
    {
        InitializeReferences();

        if (_ragdollRigidbodies.Count == 0)
        {
            Debug.LogError($"[{name}] Target Object 에 Rigidbody 가 없어 랙돌 점프스케어를 실행할 수 없습니다.", this);
            NotifyFinished();
            return;
        }

        _isDropping = true;
        _hasValidImpact = false;
        _isFinishing = false;
        _impactElapsedTime = 0.0f;
        

        for (int i = 0; i < _ragdollRigidbodies.Count; i++)
        {
            Rigidbody rigidbody = _ragdollRigidbodies[i];
            if (rigidbody == null)
            {
                continue;
            }

            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.isKinematic = false;
            rigidbody.WakeUp();
        }

        if (_enableLog == true)
        {
            Debug.Log($"[{name}] 랙돌 낙하를 시작합니다.", this);
        }
    }

    protected override void OnResetJumpScare()
    {
        if (_isInitialStateCached == false)
        {
            CacheInitialState();
        }

        _isDropping = false;
        _hasValidImpact = false;
        _isFinishing = false;
        _impactElapsedTime = 0.0f;

        for (int i = 0; i < _cachedRigidbodyStates.Count; i++)
        {
            RagdollRigidbodyState state = _cachedRigidbodyStates[i];
            if (state.Rigidbody == null)
            {
                continue;
            }

            state.Rigidbody.linearVelocity = Vector3.zero;
            state.Rigidbody.angularVelocity = Vector3.zero;
            state.Rigidbody.isKinematic = true;
            state.Rigidbody.Sleep();
        }

        _targetObject.transform.position = _initialRootWorldPosition;
        _targetObject.transform.rotation = _initialRootWorldRotation;

        for (int i = 0; i < _cachedBoneStates.Count; i++)
        {
            RagdollBoneState state = _cachedBoneStates[i];
            if (state.Transform == null)
            {
                continue;
            }

            state.Transform.localPosition = state.LocalPosition;
            state.Transform.localRotation = state.LocalRotation;
        }

        for (int i = 0; i < _cachedRigidbodyStates.Count; i++)
        {
            RagdollRigidbodyState state = _cachedRigidbodyStates[i];
            if (state.Rigidbody == null)
            {
                continue;
            }

            state.Rigidbody.isKinematic = state.IsKinematic;
        }
        
        if (_enableLog == true)
        {
            Debug.Log($"[{name}] 랙돌 점프스케어를 초기 상태로 리셋했습니다.", this);
        }
    }

    public void NotifyRagdollCollision(Collision collision)
    {
        if (_isDropping == false || _hasValidImpact == true)
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
            Debug.Log($"[{name}] 랙돌 첫 유효 충돌을 감지했습니다. 대상: {collision.gameObject.name}", this);
        }

        _onDropCollision?.Invoke();
    }

    [Button]
    public void CollectRagdollParts()
    {
        _ragdollRigidbodies.Clear();
        _ragdollBones.Clear();

        Rigidbody[] rigidbodies = TargetObject.GetComponentsInChildren<Rigidbody>(true);
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            Rigidbody rigidbody = rigidbodies[i];
            if (rigidbody == null)
            {
                continue;
            }

            _ragdollRigidbodies.Add(rigidbody);
            if (rigidbody.transform != _targetObject.transform)
            {
                _ragdollBones.Add(rigidbody.transform);
            }

            RagdollDropCollisionRelay relay = rigidbody.GetComponent<RagdollDropCollisionRelay>();
            if (relay == null)
            {
                relay = rigidbody.gameObject.AddComponent<RagdollDropCollisionRelay>();
            }

            relay.Initialize(this);
        }
    }

    private void InitializeReferences()
    {
        CollectRagdollParts();
    }
    
    private void CacheInitialState()
    {
        InitializeReferences();

        _initialRootWorldPosition = _targetObject.transform.position;
        _initialRootWorldRotation = _targetObject.transform.rotation;

        _cachedBoneStates.Clear();
        _cachedRigidbodyStates.Clear();

        for (int i = 0; i < _ragdollBones.Count; i++)
        {
            Transform bone = _ragdollBones[i];
            if (bone == null)
            {
                continue;
            }

            _cachedBoneStates.Add(new RagdollBoneState
            {
                Transform = bone,
                LocalPosition = bone.localPosition,
                LocalRotation = bone.localRotation
            });
        }

        for (int i = 0; i < _ragdollRigidbodies.Count; i++)
        {
            Rigidbody rigidbody = _ragdollRigidbodies[i];
            if (rigidbody == null)
            {
                continue;
            }

            _cachedRigidbodyStates.Add(new RagdollRigidbodyState
            {
                Rigidbody = rigidbody,
                IsKinematic = rigidbody.isKinematic
            });
        }

        _isInitialStateCached = true;
    }

    private void InitializePhysicsState()
    {
        _isDropping = false;
        _hasValidImpact = false;
        _isFinishing = false;
        _impactElapsedTime = 0.0f;

        for (int i = 0; i < _ragdollRigidbodies.Count; i++)
        {
            Rigidbody rigidbody = _ragdollRigidbodies[i];
            if (rigidbody == null)
            {
                continue;
            }

            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.isKinematic = true;
        }
    }

    private bool IsStopped()
    {
        if (_ragdollRigidbodies.Count == 0)
        {
            return true;
        }

        for (int i = 0; i < _ragdollRigidbodies.Count; i++)
        {
            Rigidbody rigidbody = _ragdollRigidbodies[i];
            if (rigidbody == null)
            {
                continue;
            }

            float linearSpeed = rigidbody.linearVelocity.magnitude;
            float angularSpeed = rigidbody.angularVelocity.magnitude;

            if (linearSpeed > _stopVelocityThreshold || angularSpeed > _stopAngularVelocityThreshold)
            {
                return false;
            }
        }

        return true;
    }

    private void FinishJumpScare()
    {
        if (_isFinishing == true)
        {
            return;
        }

        _isFinishing = true;
        _isDropping = false;

        for (int i = 0; i < _ragdollRigidbodies.Count; i++)
        {
            Rigidbody rigidbody = _ragdollRigidbodies[i];
            if (rigidbody == null)
            {
                continue;
            }

            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }

        if (_enableLog == true)
        {
            Debug.Log($"[{name}] 랙돌 점프스케어를 종료합니다.", this);
        }

        NotifyFinished();
    }

    private bool IsValidGroundCollision(Collision collision)
    {
        if (collision == null)
        {
            return false;
        }

        GameObject collisionObject = collision.gameObject;
        if (collisionObject == null)
        {
            return false;
        }

        if (IsInLayerMask(collisionObject.layer, _environmentLayerMask) == false)
        {
            return false;
        }

        if (collisionObject.CompareTag(_groundTag) == false)
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
}
