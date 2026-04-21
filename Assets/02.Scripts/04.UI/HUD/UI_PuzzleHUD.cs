using TMPro;
using UnityEngine;

public class UI_PuzzleHUD : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TextMeshProUGUI _text;

    private void OnEnable()
    {
        if (PuzzleHUDManager.Instance != null)
            PuzzleHUDManager.Instance.OnHUDChanged += Refresh;
    }

    private void OnDisable()
    {
        if (PuzzleHUDManager.Instance != null)
            PuzzleHUDManager.Instance.OnHUDChanged -= Refresh;
    }

    private void Refresh(string text, bool visible)
    {
        _root.SetActive(visible);
        if (visible)
            _text.text = text;
    }
}