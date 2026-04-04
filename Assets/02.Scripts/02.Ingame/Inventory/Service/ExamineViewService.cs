using UnityEngine;

public class ExamineViewService
{
    private readonly ItemFactory _itemFactory;
    private readonly Transform _root;

    private GameObject _currentObject;

    public string CurrentInstanceId { get; private set; }

    public ExamineViewService(ItemFactory itemFactory, Transform root)
    {
        _itemFactory = itemFactory;
        _root = root;
    }

    public GameObject Show(RuntimeItemData runtimeItemData)
    {
        Hide();

        if (runtimeItemData == null)
        {
            return null;
        }

        GameObject examineObject = _itemFactory.CreateExamineObject(runtimeItemData, _root);
        if (examineObject == null)
        {
            return null;
        }

        _itemFactory.SetLayerRecursively(examineObject, _root.gameObject.layer);
        MoveToRoot(examineObject);
        examineObject.SetActive(true);
        _currentObject = examineObject;
        CurrentInstanceId = runtimeItemData.InstanceId;
        return examineObject;
    }

    public void Hide()
    {
        if (_currentObject != null)
        {
            Object.Destroy(_currentObject);
            _currentObject = null;
        }

        CurrentInstanceId = null;
    }

    private void MoveToRoot(GameObject itemObject)
    {
        itemObject.transform.SetParent(_root, false);
        itemObject.transform.localPosition = Vector3.zero;
        itemObject.transform.localRotation = Quaternion.identity;
        itemObject.transform.localScale = Vector3.one;
    }
}