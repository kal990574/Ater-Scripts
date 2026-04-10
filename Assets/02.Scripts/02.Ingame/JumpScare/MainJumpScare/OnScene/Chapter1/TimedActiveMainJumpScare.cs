using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TimedActiveMainJumpScare : MainJumpScareBase
{
    [Header("Target")]
    [SerializeField] private GameObject _targetObject;

    [Header("Timing")]
    [SerializeField] private float _activeDuration = 2.0f;

    [Header("Option")]
    [SerializeField] private bool _forceDeactivateOnFinish = true;

    [SerializeField] private UnityEvent _activeEvent;
    [SerializeField] private UnityEvent _deactivateEvent;
    
    private Coroutine _runningCoroutine;

    protected override void OnExecute()
    {
        if (_runningCoroutine != null)
        {
            StopCoroutine(_runningCoroutine);
            _runningCoroutine = null;
        }

        _runningCoroutine = StartCoroutine(CoExecute());
    }

    private IEnumerator CoExecute()
    {
        if (_targetObject != null)
        {
            _activeEvent?.Invoke();
            _targetObject.SetActive(true);
        }

        yield return new WaitForSeconds(_activeDuration);

        if (_forceDeactivateOnFinish && _targetObject != null)
        {
            _deactivateEvent?.Invoke();
            _targetObject.SetActive(false);
        }

        _runningCoroutine = null;
        NotifyFinished();
    }

    protected void OnDisable()
    {
        if (_runningCoroutine != null)
        {
            StopCoroutine(_runningCoroutine);
            _runningCoroutine = null;
        }
    }
}