using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

//아이템의 정보 확인 및 / 조사하기 기능
public class ItemViewerUI : MonoBehaviour
{

    [SerializeField] private Transform _itemRoot;
    [SerializeField] private Camera _itemViewerCamera;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    [SerializeField] private float _rotateSpeed = 0.5f;
    [SerializeField] private float _zoomSpeed = 1f;
    [SerializeField] private float _minZoom = 100f;
    [SerializeField] private float _maxZoom = 500f;

    private Vector3 _initialCameraLocalPosition;

    private GameObject _currentItem;

    private bool _isDragging;

    private Dictionary<int, GameObject> _cache = new();

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
        if (_currentItem == null) return;

        Vector3 newPos = _itemViewerCamera.transform.position + _itemViewerCamera.transform.forward * scrollDelta * _zoomSpeed;

        float distance = Vector3.Distance(newPos, _itemRoot.position);

        if (distance >= _minZoom && distance <= _maxZoom)
        {
            _itemViewerCamera.transform.position = newPos;
        }
    }
    public void ShowItem(ItemData itemData)
    {
        if (_currentItem != null)
            _currentItem.SetActive(false);

        _itemRoot.rotation = Quaternion.identity;
        _itemViewerCamera.transform.localPosition = _initialCameraLocalPosition;

        _currentItem = GetOrCreate(itemData);
        _descriptionText.text = itemData.Description;
    }



    public void TryInteract(Vector2 screenPosition, RectTransform rawImageRect)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rawImageRect, screenPosition, null, out Vector2 localPoint);

        Vector2 viewportPoint = new Vector2(
            (localPoint.x / rawImageRect.rect.width) + 0.5f,
            (localPoint.y / rawImageRect.rect.height) + 0.5f);

        Ray ray = _itemViewerCamera.ViewportPointToRay(viewportPoint);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            TryClickInteractPoint(hit.collider.gameObject);
        }
    }
    private void TryClickInteractPoint(GameObject hitObject)
    {
        InteractPoint interactPoint = hitObject.GetComponent<InteractPoint>();
        interactPoint?.OnClick();
    }

    private GameObject GetOrCreate(ItemData itemData)
    {
        if (_cache.TryGetValue(itemData.ItemId, out GameObject cached))
        {
            cached.SetActive(true);
            return cached;
        }

        GameObject obj = Instantiate(itemData.Prefab, _itemRoot, false);
        obj.transform.localPosition = Vector3.zero;
        SetLayerRecursively(obj, _itemRoot.gameObject.layer);
        _cache[itemData.ItemId] = obj;
        return obj;
    }
    private void HandleRotate()
    {
        if (!_isDragging) return;
        if (_currentItem == null) return;

        Vector2 delta = Mouse.current.delta.ReadValue();

        _itemRoot.Rotate(Vector3.up, -delta.x * _rotateSpeed, Space.World);
        _itemRoot.Rotate(Vector3.right, delta.y * _rotateSpeed, Space.World);
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

    private void OnDisable()
    {
        _isDragging = false;

        if (_currentItem != null)
        {
            _currentItem.SetActive(false);
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
}