using System.Collections.Generic;
using UnityEngine;

public sealed class FakeEnemySpawnService
{
    private readonly Transform _spawnParent;
    private readonly bool _enableDebugLog;

    public FakeEnemySpawnService(Transform spawnParent, bool enableDebugLog)
    {
        _spawnParent = spawnParent;
        _enableDebugLog = enableDebugLog;
    }

    public bool TrySpawn(
        FakeEnemyJumpScareExecuteRequest request,
        Vector3 position,
        Quaternion rotation,
        float lifetime,
        out FakeEnemyInstance instance)
    {
        instance = null;

        GameObject selectedPrefab = SelectPrefab(request);

        if (selectedPrefab == null)
        {
            if (_enableDebugLog == true)
            {
                Debug.LogWarning("[FakeEnemySpawnService] 생성 가능한 프리팹이 없습니다.");
            }

            return false;
        }

        GameObject spawnedObject = Object.Instantiate(selectedPrefab, position, rotation, _spawnParent);
        instance = spawnedObject.GetComponent<FakeEnemyInstance>();

        if (instance == null)
        {
            if (_enableDebugLog == true)
            {
                Debug.LogWarning("[FakeEnemySpawnService] FakeEnemyInstance 컴포넌트가 프리팹에 없습니다.");
            }

            Object.Destroy(spawnedObject);
            return false;
        }

        instance.Initialize(lifetime, request.PlayerTransform);

        if (_enableDebugLog == true)
        {
            Debug.Log(string.Format(
                "[FakeEnemySpawnService] 생성 성공 | Prefab={0}",
                selectedPrefab.name));
        }

        return true;
    }

    private GameObject SelectPrefab(FakeEnemyJumpScareExecuteRequest request)
    {
        if (request.PosePrefabs == null || request.PosePrefabs.Count == 0)
        {
            return null;
        }

        List<GameObject> validPrefabs = new List<GameObject>();

        for (int index = 0; index < request.PosePrefabs.Count; index++)
        {
            GameObject prefab = request.PosePrefabs[index];

            if (prefab == null)
            {
                continue;
            }

            validPrefabs.Add(prefab);
        }

        if (validPrefabs.Count == 0)
        {
            return null;
        }

        int selectedIndex = GetRandomIndex(request, validPrefabs.Count);
        return validPrefabs[selectedIndex];
    }

    private int GetRandomIndex(FakeEnemyJumpScareExecuteRequest request, int count)
    {
        if (request.UseDeterministicSelection == true)
        {
            System.Random random = new System.Random(request.DeterministicSeed);
            return random.Next(0, count);
        }

        return UnityEngine.Random.Range(0, count);
    }
}