using TMPro;
using UnityEngine;

public class UI_AchievementCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private TextMeshProUGUI _unlockTime;
    [SerializeField] private GameObject _unlockedMarkObject;

    public void Bind(UI_AchievementViewData viewData)
    {
        if (_titleText != null)
        {
            _titleText.text = viewData.DisplayTitle;
        }

        if (_descriptionText != null)
        {
            _descriptionText.text = viewData.DisplayDescription;
        }

        if (_progressText != null)
        {
            if (viewData.IsUnlocked == true)
            {
                _progressText.text = "달성 완료";
            }
            else
            {
                _progressText.text = $"{viewData.CurrentValue} / {viewData.TargetValue}";
            }
        }

        if (_unlockTime != null)
        {
            _unlockTime.text = viewData.UnlockStatusText;
        }

        if (_unlockedMarkObject != null)
        {
            _unlockedMarkObject.SetActive(viewData.IsUnlocked);
        }
    }
}
