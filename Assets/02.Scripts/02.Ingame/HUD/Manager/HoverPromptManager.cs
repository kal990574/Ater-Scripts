using System;
using UnityEngine;

public class HoverPromptManager : MonoBehaviour
{
    public static HoverPromptManager instance {  get; private set; }

    [SerializeField] private HoverPromptDataSO _promptData;

    public event Action<string, bool> OnPromptChanged;

    private void Awake()
    {
        instance = this;
    }

    public void ShowPrompt(int[] promptIds)
    {
        string text = _promptData.BuildPromptString(promptIds);
        OnPromptChanged?.Invoke(text, true);
    }

    public void HidePrompt()
    {
        OnPromptChanged?.Invoke(string.Empty, false);
    }
}