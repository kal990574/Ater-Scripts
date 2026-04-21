using TMPro;
using UnityEngine;

public class UI_PuzzleHUD : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TextMeshProUGUI _text;

    private void OnEnable()
    {
        if (PuzzleHUDManager.instance != null)
            PuzzleHUDManager.instance.OnHUDChanged += Refresh;
    }

    private void OnDisable()
    {
        if (PuzzleHUDManager.instance != null)
            PuzzleHUDManager.instance.OnHUDChanged -= Refresh;
    }

    private void Refresh(string text, bool visible)
    {
        _root.SetActive(visible);
        if (visible)
            _text.text = text;
    }
}