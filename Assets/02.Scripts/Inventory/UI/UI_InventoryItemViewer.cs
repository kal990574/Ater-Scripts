using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_InventoryItemViewer : MonoBehaviour
{
    [SerializeField] private Transform _itemRoot;
    [SerializeField] private Camera _itemViewerCamera;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private float _rotateSpeed = 0.5f;
    [SerializeField] private float _zoomSpeed = 1f;
    [SerializeField] private float _minFieldOfView = 20f;
    [SerializeField] private float _maxFieldOfView = 60f;

    private readonly Dictionary<string, GameObject> _cache = new();

    private Vector3 _initialCameraLocalPosition;
    private GameObject _currentItem;
    private bool _isDragging;

    private void Start()
    {
        _initialCameraLocalPosition = _itemViewerCamera.transform.localPosition;
        _itemViewerCamera.transform.LookAt(_itemRoot);
    }

    private void Update()
    {
        HandleRotate();
    }

    public void SetDragging(bool isDragging)
    {
        _isDragging = isDragging;
    }

    public void Zoom(float scrollDelta)
    {
        if (_currentItem == null || _itemViewerCamera == null)
        {
            return;
        }

        float nextFieldOfView = _itemViewerCamera.fieldOfView - (scrollDelta * _zoomSpeed);
        _itemViewerCamera.fieldOfView = Mathf.Clamp(nextFieldOfView, _minFieldOfView, _maxFieldOfView);
    }

    public void ShowItem(ItemInstance itemInstance)
    {
        if (_currentItem != null)
        {
            _currentItem.SetActive(false);
        }

        _itemRoot.rotation = Quaternion.identity;
        _itemViewerCamera.transform.localPosition = _initialCameraLocalPosition;
        _itemViewerCamera.fieldOfView = _maxFieldOfView;

        _currentItem = GetOrCreate(itemInstance);
        _descriptionText.text = itemInstance != null ? itemInstance.Description : string.Empty;
    }

    public void TryInteract(Vector2 screenPosition, RectTransform rawImageRect)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImageRect, screenPosition, null, out Vector2 localPoint);

        Vector2 viewportPoint = new Vector2(
            (localPoint.x / rawImageRect.rect.width) + 0.5f,
            (localPoint.y / rawImageRect.rect.height) + 0.5f);

        Ray ray = _itemViewerCamera.ViewportPointToRay(viewportPoint);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            InteractPoint interactPoint = hit.collider.GetComponent<InteractPoint>();
            interactPoint?.OnClick();
        }
    }

    public void Hide()
    {
        if (_currentItem != null)
        {
            _currentItem.SetActive(false);
        }

        _descriptionText.text = string.Empty;
    }

    private GameObject GetOrCreate(ItemInstance itemInstance)
    {
        if (itemInstance == null || itemInstance.Prefab == null)
        {
            return null;
        }

        if (_cache.TryGetValue(itemInstance.InstanceId, out GameObject cached))
        {
            cached.SetActive(true);
            GetOrAddBinder(cached).Bind(itemInstance, InventoryManager.Instance);
            return cached;
        }

        GameObject obj = Instantiate(itemInstance.Prefab, _itemRoot, false);
        obj.transform.localPosition = Vector3.zero;
        SetLayerRecursively(obj, _itemRoot.gameObject.layer);
        GetOrAddBinder(obj).Bind(itemInstance, InventoryManager.Instance);
        _cache[itemInstance.InstanceId] = obj;
        return obj;
    }

    private ExamineItemBinder GetOrAddBinder(GameObject obj)
    {
        ExamineItemBinder binder = obj.GetComponent<ExamineItemBinder>();
        if (binder == null)
        {
            binder = obj.AddComponent<ExamineItemBinder>();
        }

        return binder;
    }

    private void HandleRotate()
    {
        if (!_isDragging || _currentItem == null)
        {
            return;
        }

        Vector2 delta = Mouse.current.delta.ReadValue();
        _itemRoot.Rotate(Vector3.up, -delta.x * _rotateSpeed, Space.World);
        _itemRoot.Rotate(Vector3.right, delta.y * _rotateSpeed, Space.World);
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private void OnDisable()
    {
        _isDragging = false;

        if (_currentItem != null)
        {
            _currentItem.SetActive(false);
        }
    }
}
