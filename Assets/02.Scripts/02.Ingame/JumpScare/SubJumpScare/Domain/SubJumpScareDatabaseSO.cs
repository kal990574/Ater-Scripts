using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SubJumpScareDatabase", menuName = "Ater/JumpScare/Sub/Database")]
public class SubJumpScareDatabaseSO : ScriptableObject
{
    [Header("Sound Definitions")]
    public List<SoundSubJumpScareDefinitionSO> SoundDefinitions = new List<SoundSubJumpScareDefinitionSO>();

    [Header("Post Process Definitions")]
    public List<PostProcessSubJumpScareDefinitionSO> PostProcessDefinitions = new List<PostProcessSubJumpScareDefinitionSO>();

    [Header("Fake Enemy Definitions")]
    public List<FakeEnemySubJumpScareDefinitionSO> FakeEnemyDefinitions = new List<FakeEnemySubJumpScareDefinitionSO>();

    // =========================
    // Runtime Cache
    // =========================

    private Dictionary<string, FakeEnemySubJumpScareDefinitionSO> _fakeEnemyDict;
    private Dictionary<string, PostProcessSubJumpScareDefinitionSO> _postProcessDict;

    private bool _isCacheBuilt;

    // =========================
    // Cache Build
    // =========================

    private void EnsureCache()
    {
        if (_isCacheBuilt == true)
        {
            return;
        }

        BuildCache();
    }

    private void BuildCache()
    {
        _fakeEnemyDict = new Dictionary<string, FakeEnemySubJumpScareDefinitionSO>();
        _postProcessDict = new Dictionary<string, PostProcessSubJumpScareDefinitionSO>();

        // Fake Enemy
        for (int i = 0; i < FakeEnemyDefinitions.Count; i++)
        {
            FakeEnemySubJumpScareDefinitionSO current = FakeEnemyDefinitions[i];

            if (IsValid(current) == false)
            {
                continue;
            }

            string id = current.Common.Id;

            if (_fakeEnemyDict.ContainsKey(id) == false)
            {
                _fakeEnemyDict.Add(id, current);
            }
            else
            {
                Debug.LogWarning($"[SubJumpScareDatabase] 중복 FakeEnemy ID 발견: {id}", this);
            }
        }

        // Post Process
        for (int i = 0; i < PostProcessDefinitions.Count; i++)
        {
            PostProcessSubJumpScareDefinitionSO current = PostProcessDefinitions[i];

            if (IsValid(current) == false)
            {
                continue;
            }

            string id = current.Common.Id;

            if (_postProcessDict.ContainsKey(id) == false)
            {
                _postProcessDict.Add(id, current);
            }
            else
            {
                Debug.LogWarning($"[SubJumpScareDatabase] 중복 PostProcess ID 발견: {id}", this);
            }
        }

        _isCacheBuilt = true;
    }

    private bool IsValid(SubJumpScareDefinitionSOBase definition)
    {
        if (definition == null)
        {
            return false;
        }

        return definition.IsValid();
    }

    // =========================
    // Public API
    // =========================

    public bool TryGetFakeEnemyDefinition(string id, out FakeEnemySubJumpScareDefinitionSO definition)
    {
        EnsureCache();

        return _fakeEnemyDict.TryGetValue(id, out definition);
    }

    public bool TryGetPostProcessDefinition(string id, out PostProcessSubJumpScareDefinitionSO definition)
    {
        EnsureCache();

        return _postProcessDict.TryGetValue(id, out definition);
    }

    // =========================
    // Editor 대응
    // =========================

#if UNITY_EDITOR
    private void OnValidate()
    {
        _isCacheBuilt = false;
    }
#endif

    private void OnEnable()
    {
        _isCacheBuilt = false;
    }
}