using UnityEngine;
using UnityEngine.EventSystems;

public class PadLockRowSelector : MonoBehaviour, IPointerClickHandler
{
    public enum RowIndex
    {
        Row1,
        Row2,
        Row3,
        Row4
    }

    [Header("Target Row")]
    [SerializeField] private RowIndex _rowIndex = RowIndex.Row1;

    [Header("Number Range")]
    [SerializeField] private int _startValue = 1;
    [SerializeField] private int _minValue = 1;
    [SerializeField] private int _maxValue = 9;

    [Header("Visual")]
    [SerializeField] private Vector3 _rotationAxis = new(0f, 0f, 1f);
    [SerializeField] private float _rotationStep = 40f;

    private PadLockPuzzleInstance _puzzleInstance;
    private int _currentValue;

    private void Awake()
    {
        _currentValue = Mathf.Clamp(_startValue, _minValue, _maxValue);
    }

    public void Bind(PadLockPuzzleInstance puzzleInstance)
    {
        _puzzleInstance = puzzleInstance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_puzzleInstance == null)
        {
            return;
        }

        RotateNext();
        _puzzleInstance.SetRowValue(_rowIndex, _currentValue);
    }

    private void RotateNext()
    {
        _currentValue++;
        if (_currentValue > _maxValue)
        {
            _currentValue = _minValue;
        }

        transform.Rotate(_rotationAxis.normalized, _rotationStep, Space.Self);
    }
}
