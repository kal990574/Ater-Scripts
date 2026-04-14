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

        _interpreters.Add(new StoryProgressAchievementInterpreter(this));
        _interpreters.Add(new CollectAchievementInterpreter(this));
        _interpreters.Add(new MechanicAchievementInterpreter(this));
        _interpreters.Add(new ChallengeAchievementInterpreter(this));
    }
}
