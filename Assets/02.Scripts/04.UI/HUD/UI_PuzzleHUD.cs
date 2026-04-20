using TMPro;
using UnityEngine;

public class UI_PuzzleHUD : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private PuzzleHUDDataSo _data;

    public void Activate(EPuzzleType puzzleType)
    {
        if (_data.BuildPuzzleHUDString(puzzleType, out string prompt))
            _text.text = prompt;
        _root.SetActive(true);
    }

    public void Deactivate()
    {
        _root.SetActive(false);
    }
}