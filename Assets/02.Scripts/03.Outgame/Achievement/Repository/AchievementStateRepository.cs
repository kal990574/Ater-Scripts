using System.Collections.Generic;
using UnityEngine;

public class AchievementStateRepository : IAchievementStateRepository
{
    private const string SaveKey = "Achievement.StateCollection";

    public List<AchievementState> LoadStates(IReadOnlyList<AchievementDefinition> definitions)
    {
        AchievementStateCollection loadedCollection = null;

        if (PlayerPrefs.HasKey(SaveKey) == true)
        {
            string json = PlayerPrefs.GetString(SaveKey, string.Empty);

            if (string.IsNullOrEmpty(json) == false)
            {
                loadedCollection = JsonUtility.FromJson<AchievementStateCollection>(json);
            }
        }

        Dictionary<AchievementId, AchievementState> loadedMap = new Dictionary<AchievementId, AchievementState>();

        if (loadedCollection != null && loadedCollection.States != null)
        {
            for (int index = 0; index < loadedCollection.States.Count; index++)
            {
                AchievementState state = loadedCollection.States[index];

                if (state == null)
                {
                    continue;
                }

                if (loadedMap.ContainsKey(state.Id) == true)
                {
                    continue;
                }

                loadedMap.Add(state.Id, state);
            }
        }

        List<AchievementState> result = new List<AchievementState>();

        for (int index = 0; index < definitions.Count; index++)
        {
            AchievementDefinition definition = definitions[index];

            if (definition == null)
            {
                continue;
            }

            if (loadedMap.TryGetValue(definition.Id, out AchievementState existingState) == true)
            {
                result.Add(existingState);
            }
            else
            {
                result.Add(new AchievementState(definition.Id));
            }
        }

        return result;
    }

    public void SaveStates(IReadOnlyList<AchievementState> states)
    {
        AchievementStateCollection collection = new AchievementStateCollection();

        for (int index = 0; index < states.Count; index++)
        {
            collection.States.Add(states[index]);
        }

        string json = JsonUtility.ToJson(collection);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }
}
