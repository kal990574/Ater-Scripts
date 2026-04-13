using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_AchievementListPage : MonoBehaviour
{
    [SerializeField] private AchievementManager _achievementManager;
    [SerializeField] private Transform _cardRoot;
    [SerializeField] private UI_AchievementCard _cardPrefab;
    [SerializeField] private TextMeshProUGUI _progressSummaryText;

    private readonly List<UI_AchievementCard> _spawnedCards = new List<UI_AchievementCard>();

    private void OnEnable()
    {
        if (_achievementManager != null)
        {
            _achievementManager.AchievementListChanged += Refresh;
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (_achievementManager != null)
        {
            _achievementManager.AchievementListChanged -= Refresh;
        }
    }

    public void Refresh()
    {
        ClearCards();

        if (_achievementManager == null || _cardRoot == null || _cardPrefab == null)
        {
            return;
        }

        IReadOnlyList<AchievementDefinition> definitions = _achievementManager.GetAllDefinitions();

        for (int index = 0; index < definitions.Count; index++)
        {
            AchievementDefinition definition = definitions[index];

            if (definition == null)
            {
                continue;
            }

            if (_achievementManager.TryGetState(definition.Id, out AchievementState state) == false)
            {
                continue;
            }

            UI_AchievementViewData viewData = CreateViewData(definition, state);

            if (viewData.IsVisible == false)
            {
                continue;
            }

            UI_AchievementCard card = Instantiate(_cardPrefab, _cardRoot);
            card.Bind(viewData);
            _spawnedCards.Add(card);
        }

        if (_progressSummaryText != null)
        {
            _progressSummaryText.text = $"{_achievementManager.GetUnlockedCount()} / {_achievementManager.GetTotalCount()} 달성";
        }
    }

    private UI_AchievementViewData CreateViewData(AchievementDefinition definition, AchievementState state)
    {
        bool isHiddenAndLocked =
            definition.VisibilityType == EAchievementVisibilityType.Hidden &&
            state.IsUnlocked == false;

        if (isHiddenAndLocked == true)
        {
            return new UI_AchievementViewData(
                definition.Id,
                string.Empty,
                string.Empty,
                state.IsUnlocked,
                false,
                state.CurrentValue,
                definition.TargetValue,
                definition.Category);
        }

        string displayTitle = state.IsUnlocked == true ? definition.Title : "???";

        return new UI_AchievementViewData(
            definition.Id,
            displayTitle,
            definition.Description,
            state.IsUnlocked,
            true,
            state.CurrentValue,
            definition.TargetValue,
            definition.Category);
    }

    private void ClearCards()
    {
        for (int index = 0; index < _spawnedCards.Count; index++)
        {
            if (_spawnedCards[index] != null)
            {
                Destroy(_spawnedCards[index].gameObject);
            }
        }

        _spawnedCards.Clear();
    }
}
