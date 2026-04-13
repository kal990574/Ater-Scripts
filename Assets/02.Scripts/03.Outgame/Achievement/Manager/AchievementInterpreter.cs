// AchievementInterpreter.cs
using System.Collections.Generic;
using UnityEngine;

public class AchievementInterpreter : MonoBehaviour
{
    private readonly List<IAchievementSubInterpreter> _interpreters = new List<IAchievementSubInterpreter>();

    private void Awake()
    {
        RegisterInterpreters();
    }

    private void OnEnable()
    {
        for (int index = 0; index < _interpreters.Count; index++)
        {
            _interpreters[index].Enable();
        }
    }

    private void OnDisable()
    {
        for (int index = 0; index < _interpreters.Count; index++)
        {
            _interpreters[index].Disable();
        }
    }

    private void RegisterInterpreters()
    {
        _interpreters.Clear();

        _interpreters.Add(new OnPrologueClearedAchievementInterpreter(this));
        _interpreters.Add(new OnAct1ClearedAchievementInterpreter(this));
        _interpreters.Add(new OnAct2ClearedAchievementInterpreter(this));
        _interpreters.Add(new OnAct3ClearedAchievementInterpreter(this));
        _interpreters.Add(new OnEndingReachedAchievementInterpreter(this));

        _interpreters.Add(new OnFirstLogAchievementInterpreter(this));
        _interpreters.Add(new OnAllTextLogsCollectedAchievementInterpreter(this));

        _interpreters.Add(new OnFirstSignalAchievementInterpreter(this));
        _interpreters.Add(new OnRestorationExpertAchievementInterpreter(this));
        _interpreters.Add(new OnChatterboxAchievementInterpreter(this));
        _interpreters.Add(new OnAdaptedToDarknessAchievementInterpreter(this));

        _interpreters.Add(new OnNoAiClearAchievementInterpreter(this));
    }
}