using UnityEngine;

public class SpawnJumpScare : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Prefab")]
    [SerializeField] private GameObject prefabToSpawn;

    [Header("Spawn Option")]
    [SerializeField] private bool useTargetPosition = true;
    [SerializeField] private Vector3 localOffset = Vector3.zero;
    [SerializeField] private Vector3 worldOffset = Vector3.zero;
    [SerializeField] private bool useTargetRotation = true;
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;
    [SerializeField] private Transform parentAfterSpawn;

    public Transform Target => target;
    public GameObject PrefabToSpawn => prefabToSpawn;

    public bool TrySpawn(out GameObject spawnedObject)
    {
        spawnedObject = null;

        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"{nameof(SpawnJumpScare)} : Spawn할 프리팹이 비어 있습니다.", this);
            return false;
        }

        if (target == null)
        {
            Debug.LogWarning($"{nameof(SpawnJumpScare)} : 기준 타겟이 비어 있습니다.", this);
            return false;
        }

        Vector3 spawnPosition = GetSpawnPosition();
        Quaternion spawnRotation = GetSpawnRotation();

        spawnedObject = Instantiate(prefabToSpawn, spawnPosition, spawnRotation);

        if (parentAfterSpawn != null)
        {
            spawnedObject.transform.SetParent(parentAfterSpawn, true);
        }

        return true;
    }

    [ContextMenu("Spawn Prefab")]
    public void Spawn()
    {
        TrySpawn(out _);
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 spawnPosition = Vector3.zero;

        if (useTargetPosition == true)
        {
            spawnPosition = target.position;
            spawnPosition += target.TransformDirection(localOffset);
        }

        spawnPosition += worldOffset;

        return spawnPosition;
    }

    private Quaternion GetSpawnRotation()
    {
        Quaternion spawnRotation = Quaternion.identity;

        if (useTargetRotation == true)
        {
            spawnRotation = target.rotation;
        }

        spawnRotation *= Quaternion.Euler(rotationOffset);

        return spawnRotation;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetPrefab(GameObject newPrefab)
    {
        prefabToSpawn = newPrefab;
    }
}