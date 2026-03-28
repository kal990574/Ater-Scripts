using UnityEngine;

//이것 자체가 키
[CreateAssetMenu(fileName = "StateKey", menuName = "Ater/Inventory/Item/State Key")]
public class StateKeySO : ScriptableObject
{
    [SerializeField] private string _persistentKey;

    public string PersistentKey => string.IsNullOrEmpty(_persistentKey) ? name : _persistentKey;
}
