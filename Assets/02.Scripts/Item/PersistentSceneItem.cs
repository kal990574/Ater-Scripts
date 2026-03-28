using System;
using UnityEngine;

[DisallowMultipleComponent]
public class PersistentSceneItem : MonoBehaviour
{
    [SerializeField] private string _sceneObjectId;
    [SerializeField] private InstanceView _instanceView;
    [SerializeField] private bool _disableWhenCollected = true;

    public string SceneObjectId => _sceneObjectId;
    public InstanceView InstanceView => _instanceView;
    public bool DisableWhenCollected => _disableWhenCollected;

    private void Awake()
    {
        if (_instanceView == null)
        {
            _instanceView = GetComponent<InstanceView>();
        }
    }

    private void OnEnable()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RegisterSceneItem(this);
        }
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.UnregisterSceneItem(this);
        }
    }

    public void MarkCollected(string instanceId)
    {
        if (InventoryManager.Instance == null)
        {
            return;
        }

        InventoryManager.Instance.MarkSceneItemCollected(_sceneObjectId, instanceId);
    }

    public void ApplyCollectedState(bool isCollected)
    {
        if (!_disableWhenCollected)
        {
            return;
        }

        gameObject.SetActive(!isCollected);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_instanceView == null)
        {
            _instanceView = GetComponent<InstanceView>();
        }

        if (string.IsNullOrEmpty(_sceneObjectId))
        {
            _sceneObjectId = Guid.NewGuid().ToString("N");
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}
