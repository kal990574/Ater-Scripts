using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class UI_AchievementListPage : MonoBehaviour
{
    [SerializeField] private Transform _cardRoot;
    [SerializeField] private UI_AchievementCard _cardPrefab;
    [SerializeField] private TextMeshProUGUI _progressSummaryText;

    private readonly List<UI_AchievementCard> _spawnedCards = new List<UI_AchievementCard>();
    

    private void OnEnable()
    {
        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.AchievementListChanged += Refresh;
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.AchievementListChanged -= Refresh;
        }
    }

    public void Refresh()
    {
        ClearCards();

        if (AchievementManager.Instance == null || _cardRoot == null || _cardPrefab == null)
        {
            return;
        }

        IReadOnlyList<AchievementDefinition> definitions = AchievementManager.Instance.GetAllDefinitions();

        for (int index = 0; index < definitions.Count; index++)
        {
            AchievementDefinition definition = definitions[index];

            if (definition == null)
            {
                continue;
            }

            if (AchievementManager.Instance.TryGetState(definition.Id, out AchievementState state) == false)
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
            _progressSummaryText.text = $"{AchievementManager.Instance.GetUnlockedCount()} / {AchievementManager.Instance.GetTotalCount()} 달성";
        }
    }

    private UI_AchievementViewData CreateViewData(AchievementDefinition definition, AchievementState state)
    {
        int displayCurrentValue = AchievementManager.Instance != null
            ? AchievementManager.Instance.GetDisplayCurrentValue(definition, state)
            : state != null ? state.CurrentValue : 0;

        bool isHiddenAndLocked =
            definition.VisibilityType == EAchievementVisibilityType.Hidden &&
            state.IsUnlocked == false;

        if (isHiddenAndLocked == true)
        {
            return new UI_AchievementViewData(
                definition.Id,
                string.Empty,
                string.Empty,
                "미달성",
                state.IsUnlocked,
                false,
                displayCurrentValue,
                definition.TargetValue,
                definition.Category);
        }

        string displayTitle = state.IsUnlocked == true ? definition.Title : "???";

        return new UI_AchievementViewData(
            definition.Id,
            displayTitle,
            definition.Description,
            GetUnlockStatusText(state),
            state.IsUnlocked,
            true,
            displayCurrentValue,
            definition.TargetValue,
            definition.Category);
    }

    private static string GetUnlockStatusText(AchievementState state)
    {
        if (state == null || state.IsUnlocked == false || state.UnlockedAtUnixSeconds <= 0)
        {
            return "미달성";
        }

        string formattedTime = DateTimeOffset
            .FromUnixTimeSeconds(state.UnlockedAtUnixSeconds)
            .ToLocalTime()
            .ToString("MMMMd,yyyy 'at' h:mm tt", CultureInfo.InvariantCulture);

        return $"달성시간 : {formattedTime}";
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
