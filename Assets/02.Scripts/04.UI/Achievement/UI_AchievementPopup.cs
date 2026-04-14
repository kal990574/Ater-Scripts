using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AchievementPopup : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _titleText;

    public void Bind(AchievementDefinition definition)
    {
        if (_icon != null)
        {
            _icon.sprite = definition.Icon;
        }
        if (_titleText != null)
        {
            _titleText.text = definition.Title;
        }
    }
}
