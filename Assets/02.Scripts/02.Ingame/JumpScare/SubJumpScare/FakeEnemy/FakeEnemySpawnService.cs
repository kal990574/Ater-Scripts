using UnityEngine;

public class FakeEnemySpawnService : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject _fakeEnemyPrefab;
    [SerializeField] private Transform _spawnParent;
    [SerializeField] private bool _enableDebugLog = false;

    public bool TrySpawn(
        Vector3 position,
        Quaternion rotation,
        float lifetime,
        out FakeEnemyInstance instance)
    {
        instance = null;

        if (_fakeEnemyPrefab == null)
        {
            if (_enableDebugLog == true)
            {
                Debug.LogWarning("[FakeEnemySpawnService] 가짜적 프리팹이 설정되지 않았습니다.");
            }

            return false;
        }

        GameObject spawnedObject = Instantiate(_fakeEnemyPrefab, position, rotation, _spawnParent);

        instance = GetOrAddInstanceComponent(spawnedObject);
        instance.Initialize(lifetime);

        if (_enableDebugLog == true)
        {
            Debug.Log(string.Format(
                "[FakeEnemySpawnService] 가짜적 생성 완료. Position: {0}",
                position));
        }

        return true;
    }

    private FakeEnemyInstance GetOrAddInstanceComponent(GameObject spawnedObject)
    {
        FakeEnemyInstance instance = spawnedObject.GetComponent<FakeEnemyInstance>();

        if (instance != null)
        {
            return instance;
        }

        return spawnedObject.AddComponent<FakeEnemyInstance>();
    }
}