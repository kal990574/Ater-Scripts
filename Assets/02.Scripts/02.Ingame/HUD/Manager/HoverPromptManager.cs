using System;
using UnityEngine;

public class HoverPromptManager : MonoBehaviour
{
    public static HoverPromptManager Instance {  get; private set; }

    [SerializeField] private HoverPromptDataSO _promptData;

    public event Action<string, bool> OnPromptChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowPrompt(int[] promptIds)
    {
        string text = _promptData.BuildPromptString(promptIds);
        OnPromptChanged?.Invoke(text, !string.IsNullOrEmpty(text));
    }

    public void HidePrompt()
    {
        OnPromptChanged?.Invoke(string.Empty, false);
    }
}