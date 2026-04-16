using _02.Scripts.Player;
using System.Collections.Generic;
using UnityEngine;

public class SubJumpScareSelectionCoordinator
{
    private readonly SubJumpScareCommonValidator _commonValidator;
    private readonly SubJumpScareCandidateCollector _candidateCollector;
    private readonly SubJumpScareWeightedPicker _weightedPicker;
    private readonly SubJumpScareCooldownState _cooldownState;
    private readonly SubJumpScareHistory _history;

    private bool _fakeEnemyGuaranteePending;

    public bool FakeEnemyGuaranteePending
    {
        get
        {
            return _fakeEnemyGuaranteePending;
        }
    }

    public float GlobalCooldownRemaining => _cooldownState.GetRemainingGlobalCooldown();

    public SubJumpScareSelectionCoordinator(
        SubJumpScareCommonValidator commonValidator,
        SubJumpScareCandidateCollector candidateCollector,
        SubJumpScareWeightedPicker weightedPicker,
        SubJumpScareCooldownState cooldownState,
        SubJumpScareHistory history)
    {
        _commonValidator = commonValidator;
        _candidateCollector = candidateCollector;
        _weightedPicker = weightedPicker;
        _cooldownState = cooldownState;
        _history = history;
    }

    public SubJumpScareSelectionResult SelectPeriodic(
        SubJumpScareDatabaseSO database,
        SubJumpScareContext context)
    {
        string commonBlockReason;

        if (_commonValidator.TryGetBlockReason(database, context, _cooldownState, out commonBlockReason) == true)
        {
            return SubJumpScareSelectionResult.CreateFail(ESubJumpScareTriggerType.Periodic, commonBlockReason);
        }

        SubJumpScareSelectionResult postResult = TrySelectPostProcess(database, context);

        if (postResult.IsSuccess == true)
        {
            return postResult;
        }

        SubJumpScareSelectionResult soundResult = TrySelectSound(database, context);

        if (soundResult.IsSuccess == true)
        {
            return soundResult;
        }

        return SubJumpScareSelectionResult.CreateFail(
            ESubJumpScareTriggerType.Periodic,
            "주기 검사에서 선택 가능한 포스트/사운드 후보가 없습니다.");
    }

    public SubJumpScareSelectionResult SelectSonar(
        SubJumpScareDatabaseSO database,
        SubJumpScareContext context)
    {
        string commonBlockReason;

        if (_commonValidator.TryGetBlockReason(database, context, _cooldownState, out commonBlockReason) == true)
        {
            return SubJumpScareSelectionResult.CreateFail(ESubJumpScareTriggerType.Sonar, commonBlockReason);
        }

        if (IsBlockedPlayerModeForFakeEnemy(context.CurrentPlayerInteractMode) == true)
        {
            return SubJumpScareSelectionResult.CreateFail(
                ESubJumpScareTriggerType.Sonar,
                "현재 플레이어 모드에서는 가짜적을 사용할 수 없습니다.");
        }

        SubJumpScareSelectionResult fakeEnemyResult = TrySelectFakeEnemy(database, context);

        if (fakeEnemyResult.IsSuccess == true)
        {
            return fakeEnemyResult;
        }

        _fakeEnemyGuaranteePending = true;
        return fakeEnemyResult;
    }

    public void ConfirmPeriodicTriggered(SubJumpScareSelectionResult result)
    {
        if (result.IsSuccess == false)
        {
            return;
        }

        if (result.Data == null)
        {
            return;
        }

        Commit(result);
    }

    public void ConfirmSonarTriggered(SubJumpScareSelectionResult result)
    {
        if (result.IsSuccess == false)
        {
            return;
        }

        if (result.Data == null)
        {
            return;
        }

        if (result.Data.Type == ESubJumpScareType.FakeEnemy)
        {
            _fakeEnemyGuaranteePending = false;
        }

        Commit(result);
    }

    public void KeepFakeEnemyGuaranteePending()
    {
        _fakeEnemyGuaranteePending = true;
    }

    public float GetTypeCooldownRemaining(ESubJumpScareType type)
    {
        return _cooldownState.GetRemainingTypeCooldown(type);
    }

    public List<SubJumpScareItemCooldownDebugInfo> GetActiveItemCooldowns()
    {
        return _cooldownState.GetActiveItemCooldowns();
    }

    private SubJumpScareSelectionResult TrySelectPostProcess(
        SubJumpScareDatabaseSO database,
        SubJumpScareContext context)
    {
        if (_cooldownState.IsTypeCooldownActive(ESubJumpScareType.PostProcess) == true)
        {
            return SubJumpScareSelectionResult.CreateFail(
                ESubJumpScareTriggerType.Periodic,
                "포스트 프로세스 타입 쿨다운 중입니다.");
        }

        List<PostProcessSubJumpScareDefinitionSO> candidates =
            _candidateCollector.GetPostProcessCandidates(database, context, _cooldownState, _history);

        PostProcessSubJumpScareDefinitionSO selected =
            _weightedPicker.SelectWeighted(candidates, item => item.Common.Weight);

        if (selected == null)
        {
            return SubJumpScareSelectionResult.CreateFail(
                ESubJumpScareTriggerType.Periodic,
                "선택 가능한 포스트 프로세스 정의가 없습니다.");
        }

        return SubJumpScareSelectionResult.CreateSuccess(
            ESubJumpScareTriggerType.Periodic,
            selected.Common);
    }

    private SubJumpScareSelectionResult TrySelectSound(
        SubJumpScareDatabaseSO database,
        SubJumpScareContext context)
    {
        if (_cooldownState.IsTypeCooldownActive(ESubJumpScareType.Sound) == true)
        {
            return SubJumpScareSelectionResult.CreateFail(
                ESubJumpScareTriggerType.Periodic,
                "사운드 타입 쿨다운 중입니다.");
        }

        List<SoundSubJumpScareDefinitionSO> candidates =
            _candidateCollector.GetSoundCandidates(database, context, _cooldownState, _history);

        SoundSubJumpScareDefinitionSO selected =
            _weightedPicker.SelectWeighted(candidates, item => item.Common.Weight);

        if (selected == null)
        {
            return SubJumpScareSelectionResult.CreateFail(
                ESubJumpScareTriggerType.Periodic,
                "선택 가능한 사운드 정의가 없습니다.");
        }

        return SubJumpScareSelectionResult.CreateSuccess(
            ESubJumpScareTriggerType.Periodic,
            selected.Common);
    }

    private SubJumpScareSelectionResult TrySelectFakeEnemy(
        SubJumpScareDatabaseSO database,
        SubJumpScareContext context)
    {
        if (_cooldownState.IsTypeCooldownActive(ESubJumpScareType.FakeEnemy) == true)
        {
            return SubJumpScareSelectionResult.CreateFail(
                ESubJumpScareTriggerType.Sonar,
                "가짜적 타입 쿨다운 중입니다.");
        }

        List<FakeEnemySubJumpScareDefinitionSO> candidates =
            _candidateCollector.GetFakeEnemyCandidates(database, context, _cooldownState, _history);

        FakeEnemySubJumpScareDefinitionSO selected =
            _weightedPicker.SelectWeighted(candidates, item => item.Common.Weight);

        if (selected == null)
        {
            return SubJumpScareSelectionResult.CreateFail(
                ESubJumpScareTriggerType.Sonar,
                "선택 가능한 가짜적 정의가 없습니다.");
        }

        return SubJumpScareSelectionResult.CreateSuccess(
            ESubJumpScareTriggerType.Sonar,
            selected.Common);
    }

    private void Commit(SubJumpScareSelectionResult result)
    {
        _cooldownState.Commit(result);
        _history.Record(result);
    }

    private bool IsBlockedPlayerModeForFakeEnemy(EPlayerInteractMode interactMode)
    {
        return interactMode == EPlayerInteractMode.UI
            || interactMode == EPlayerInteractMode.Puzzle;
    }
}
