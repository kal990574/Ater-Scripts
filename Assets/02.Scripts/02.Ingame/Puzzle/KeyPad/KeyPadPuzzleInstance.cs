using UnityEngine;

[DisallowMultipleComponent]
public class KeyPadPuzzleInstance : MonoBehaviour
{
    private KeyPadController _controller;
    private string _correctCode;

    public void Initialize(KeyPadController controller, string correctCode)
    {
        _controller = controller;
        _correctCode = correctCode;
    }

    public void TryEvaluate()
    {
        Debug.Log($"[{nameof(KeyPadPuzzleInstance)}] TryEvaluate called on {gameObject.name}. Implement world keypad evaluation here. expectedCode={_correctCode}", this);
    }

    [ContextMenu("Complete Puzzle")]
    public void CompletePuzzle()
    {
        _controller?.HandlePuzzleSuccess(this);
        Destroy(gameObject);
    }

    [ContextMenu("Fail Puzzle")]
    public void FailPuzzle()
    {
        _controller?.HandlePuzzleFail(this);
    }

    public void Cancel()
    {
        _controller?.ClearActiveInstance(this);
        Destroy(gameObject);
    }
}
