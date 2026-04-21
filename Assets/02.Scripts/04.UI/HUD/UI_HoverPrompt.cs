using TMPro;
using UnityEngine;

public class UI_HoverPrompt : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TextMeshProUGUI _text;

    private void OnEnable()
    {
        if (HoverPromptManager.Instance != null)
            HoverPromptManager.Instance.OnPromptChanged += Refresh;
    }

    private void OnDisable()
    {
        if (HoverPromptManager.Instance != null)
            HoverPromptManager.Instance.OnPromptChanged -= Refresh;
    }

    private void Refresh(string text, bool visible)
    {
        _root.SetActive(visible);
        if(visible)
            _text.text = text;
    }
}