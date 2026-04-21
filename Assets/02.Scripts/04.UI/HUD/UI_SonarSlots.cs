using DG.Tweening;
using System.Collections.Generic;
using System;
using _02.Scripts.Sonar;
using UnityEngine;

public class UI_SonarSlots : MonoBehaviour
{
    [SerializeField] private List<CanvasGroup> _slots = new();

    [SerializeField] private float _activeAlpha = 1f;
    [SerializeField] private float _inactiveAlpha = 0.4f;

    [Header("Fail Shake")]
    [SerializeField] private float _failPunchStrength = 24f;
    [SerializeField] private float _failPunchDuration = 0.35f;
    [SerializeField] private int _failPunchVibrato = 18;
    [SerializeField] private float _failPunchElasticity = 0.9f;
    
    [Header("Binding")]
    [SerializeField] private SonarScanFeature _sonarScanFeature;
    [SerializeField]private RectTransform _rectTransform;
    
    private int _currentCount = 0;
    private Tween _failPunchTween;

    public int MaxCount => _slots.Count;
    public int CurrentCount => _currentCount;

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //        UseCount();

    //    if (Input.GetKeyDown(KeyCode.F))
    //        AddCount();
    //}
    private void Start()
    {
        if (_sonarScanFeature != null)
        {
            SetCount(_sonarScanFeature.CurrentCharges);
            _sonarScanFeature.OnChargesChanged += OnChargesChanged;
            _sonarScanFeature.OnScanFailed += OnScanFailed;
        }
        else
        {
            SetCount(MaxCount);
        }
        
        RefreshSlot();
    }

    private void OnChargesChanged(int current, int max)
    {
        SetCount(current);
    }

    private void OnDestroy()
    {
        if (_sonarScanFeature != null)
        {
            _sonarScanFeature.OnChargesChanged -= OnChargesChanged;
            _sonarScanFeature.OnScanFailed -= OnScanFailed;
        }

        _failPunchTween?.Kill();
    }

    private void OnScanFailed()
    {
        if (_rectTransform == null)
        {
            return;
        }

        _failPunchTween?.Kill();
        _rectTransform.anchoredPosition = new Vector2(0f, _rectTransform.anchoredPosition.y);
        _failPunchTween = _rectTransform
            .DOPunchAnchorPos(
                new Vector2(_failPunchStrength, 0f),
                _failPunchDuration,
                _failPunchVibrato,
                _failPunchElasticity)
            .SetLink(gameObject);
    }

    public void SetCount(int count)
    {
        _currentCount = Mathf.Clamp(count, 0, MaxCount);
        RefreshSlot();
    }

    public void AddCount(int amount = 1)
    {
        SetCount(_currentCount + amount);
    }

    public void UseCount(int amount = 1)
    {
        SetCount(_currentCount - amount);
    }

    private void RefreshSlot()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i] == null) continue;

            _slots[i].alpha = i < _currentCount ? _activeAlpha : _inactiveAlpha;
        }
    }
}
