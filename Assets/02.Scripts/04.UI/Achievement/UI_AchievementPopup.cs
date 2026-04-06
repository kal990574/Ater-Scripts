using TMPro;
using UnityEngine;

public class UI_AchievementPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _messageText;

    public void Bind(AchievementDefinition definition)
    {
        if (_titleText != null)
        {
            _titleText.text = definition.Title;
        }

        if (_messageText != null)
        {
            _messageText.text = "업적 달성!";
        }
    }
}
