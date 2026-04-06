using System.Collections.Generic;
using UnityEngine;
using _02.Scripts.Sonar;
using System;

public class UI_SonarSlots : MonoBehaviour
{
    [SerializeField] private List<CanvasGroup> _slots = new();

    [SerializeField] private float _activeAlpha = 1f;
    [SerializeField] private float _inactiveAlpha = 0.4f;
    
    [Header("Binding")]
    [SerializeField] private SonarScanFeature _sonarScanFeature;

    private int _currentCount = 0;

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
        }
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
