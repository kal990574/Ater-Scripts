using TMPro;
using UnityEngine;

public class UI_HoverPrompt : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private HoverPromptManager _manager;

    private void OnEnable()
    {
        if (_manager != null)
            _manager.OnPromptChanged += Refresh;
    }

    private void OnDisable()
    {
        if (_manager != null)
            _manager.OnPromptChanged -= Refresh;
    }

    private void Refresh(string text, bool visible)
    {
        _root.SetActive(visible);
        if(visible)
            _text.text = text;
    }
}