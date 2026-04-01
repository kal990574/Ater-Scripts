using UnityEngine;

[DisallowMultipleComponent]
//추후 공용 추상스크립트 제작
public class PadLockPuzzleInstance : MonoBehaviour
{
    [Header("Rows")]
    [SerializeField] private int _row1 = 1;
    [SerializeField] private int _row2 = 1;
    [SerializeField] private int _row3 = 1;
    [SerializeField] private int _row4 = 1;

    [Header("Evaluation")]
    [SerializeField] private bool _checkOnRowClick = false;
    [SerializeField] private bool _invokeFailOnWrongCode = true;
    [SerializeField] private bool _destroyOnSuccess = true;

    private PadLockController _owner;
    private string _correctCode;
    private bool _isSolved;
    private bool _isInitializing;

    public void Initialize(PadLockController owner, string correctCode)
    {
        _owner = owner;
        _correctCode = correctCode ?? string.Empty;
        _isInitializing = true;

        PadLockRowSelector[] rowSelectors = GetComponentsInChildren<PadLockRowSelector>(true);
        foreach (PadLockRowSelector rowSelector in rowSelectors)
        {
            rowSelector.Bind(this);
        }

        _isInitializing = false;
    }

    public void SetRowValue(PadLockRowSelector.RowIndex rowIndex, int value)
    {
        switch (rowIndex)
        {
            case PadLockRowSelector.RowIndex.Row1:
                _row1 = value;
                break;
            case PadLockRowSelector.RowIndex.Row2:
                _row2 = value;
                break;
            case PadLockRowSelector.RowIndex.Row3:
                _row3 = value;
                break;
            case PadLockRowSelector.RowIndex.Row4:
                _row4 = value;
                break;
        }
    }

    public void TryEvaluate()
    {
        if (_isSolved || _owner == null)
        {
            return;
        }

        if (BuildCurrentCode() == _correctCode)
        {
            _isSolved = true;
            _owner.HandlePuzzleSuccess(this);

            if (_destroyOnSuccess)
            {
                Destroy(gameObject);
            }
            return;
        }

        if (_invokeFailOnWrongCode)
        {
            _owner.HandlePuzzleFail(this);
        }
    }

    public void Cancel()
    {
        if (_isSolved)
        {
            return;
        }

        Destroy(gameObject);
    }

    private string BuildCurrentCode()
    {
        return $"{_row1:0}{_row2:0}{_row3:0}{_row4:0}";
    }

    private void OnDestroy()
    {
        if (_owner != null && !_isSolved)
        {
            _owner.ClearActiveInstance(this);
        }
    }
}
