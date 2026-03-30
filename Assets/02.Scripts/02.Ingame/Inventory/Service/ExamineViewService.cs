using System.Collections.Generic;
using UnityEngine;

public class ExamineViewService
{
    private readonly Dictionary<string, GameObject> _cache = new();
    private readonly ItemFactory _itemFactory;
    private readonly Transform _root;

    private GameObject _currentObject;

    public string CurrentInstanceId { get; private set; }

    public ExamineViewService(ItemFactory itemFactory, Transform root)
    {
        _itemFactory = itemFactory;
        _root = root;
    }

    public GameObject Show(string instanceId)
    {
        Hide();
        GameObject examineObject = GetOrCreate(instanceId);
        if (examineObject == null)
        {
            return null;
        }

        _itemFactory.SetLayerRecursively(examineObject, _root.gameObject.layer);
        examineObject.SetActive(true);
        _currentObject = examineObject;
        CurrentInstanceId = instanceId;
        return examineObject;
    }

    public void Hide()
    {
        if (_currentObject == null)
        {
            CurrentInstanceId = null;
            return;
        }

        MoveToRoot(_currentObject);
        _currentObject.SetActive(false);
        _currentObject = null;
        CurrentInstanceId = null;
    }

    public void Remove(string instanceId)
    {
        if (string.IsNullOrEmpty(instanceId))
        {
            return;
        }

        if (_cache.Remove(instanceId, out GameObject cached) && cached != null)
        {
            if (_currentObject == cached)
            {
                _currentObject = null;
                CurrentInstanceId = null;
            }

            Object.Destroy(cached);
        }
    }

    private GameObject GetOrCreate(string instanceId)
    {
        if (string.IsNullOrEmpty(instanceId))
        {
            return null;
        }

        if (_cache.TryGetValue(instanceId, out GameObject cached) && cached != null)
        {
            _itemFactory.Bind(cached, instanceId);
            return cached;
        }

        GameObject created = _itemFactory.CreateExamineObject(instanceId, _root);
        if (created == null)
        {
            return null;
        }
        MoveToRoot(created);
        created.SetActive(false);
        _cache[instanceId] = created;
        return created;
    }

    private void MoveToRoot(GameObject itemObject)
    {
        itemObject.transform.SetParent(_root, false);
        itemObject.transform.localPosition = Vector3.zero;
        itemObject.transform.localRotation = Quaternion.identity;
        itemObject.transform.localScale = Vector3.one;
    }
}
