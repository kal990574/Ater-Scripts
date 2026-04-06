using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MainJumpScareTriggerZone_Once : MainJumpScareActivator
{
    [Header("Trigger Option")]
    [SerializeField] private bool _disableColliderAfterTrigger = true;

    private Collider _triggerCollider;
    private bool _hasTriggered;

    private void Awake()
    {
        _triggerCollider = GetComponent<Collider>();

        if (_triggerCollider != null)
        {
            _triggerCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered == true)
        {
            return;
        }

        if (other == null)
        {
            return;
        }

        if (other.CompareTag("Player") == false)
        {
            return;
        }

        bool isActivated = TryActivate();
        if (isActivated == false)
        {
            return;
        }

        _hasTriggered = true;

        if (_enableLog == true)
        {
            Debug.Log($"[{name}] 1회성 메인 점프스케어 트리거가 소모되었습니다.", this);
        }

        if (_disableColliderAfterTrigger == true && _triggerCollider != null)
        {
            _triggerCollider.enabled = false;
        }
    }
}