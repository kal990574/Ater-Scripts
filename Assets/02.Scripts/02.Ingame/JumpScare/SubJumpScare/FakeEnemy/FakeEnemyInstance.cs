using UnityEngine;

public class FakeEnemyInstance : MonoBehaviour
{
    [Header("Runtime")]
    [SerializeField] private float _remainingLifetime;
    [SerializeField] private bool _isInitialized;

    [SerializeField] private bool _useAnimation;
    [SerializeField] private bool _isRandomAnimationTime;
    [SerializeField] private bool _isMove;
    [SerializeField] private float _moveSpeed;

    private Transform _targetTransform;
    private Animator _animator;

    public void Initialize(float lifetime, Transform targetTransform)
    {
        _remainingLifetime = Mathf.Max(0.0f, lifetime);
        _targetTransform = targetTransform;
        _isInitialized = true;

        CacheComponents();
        ApplyAnimationOption();
    }

    private void Update()
    {
        if (_isInitialized == false)
        {
            return;
        }

        UpdateLifetime();
        UpdateMove();
    }

    private void CacheComponents()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void UpdateLifetime()
    {
        _remainingLifetime -= Time.deltaTime;

        if (_remainingLifetime > 0.0f)
        {
            return;
        }

        Destroy(gameObject);
    }

    private void UpdateMove()
    {
        if (_isMove == false)
        {
            return;
        }

        if (_targetTransform == null)
        {
            return;
        }

        if (_moveSpeed <= 0.0f)
        {
            return;
        }

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = _targetTransform.position;
        Vector3 moveDirection = targetPosition - currentPosition;
        moveDirection.y = 0.0f;

        if (moveDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        Vector3 normalizedDirection = moveDirection.normalized;
        Vector3 nextPosition = currentPosition + (normalizedDirection * _moveSpeed * Time.deltaTime);

        transform.position = nextPosition;
        transform.rotation = Quaternion.LookRotation(normalizedDirection, Vector3.up);
    }

    private void ApplyAnimationOption()
    {
        if (_animator == null)
        {
            return;
        }

        _animator.enabled = _useAnimation;

        if (_useAnimation == false)
        {
            return;
        }

        if (_isRandomAnimationTime == false)
        {
            return;
        }

        PlayAnimationAtRandomTime();
    }

    private void PlayAnimationAtRandomTime()
    {
        _animator.Update(0.0f);

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.length <= 0.0f)
        {
            return;
        }

        float randomNormalizedTime = Random.Range(0.0f, 1.0f);
        _animator.Play(stateInfo.fullPathHash, 0, randomNormalizedTime);
        _animator.Update(0.0f);
    }
}