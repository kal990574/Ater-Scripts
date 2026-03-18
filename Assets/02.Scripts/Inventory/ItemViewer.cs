using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemViewer : MonoBehaviour
{
    public static ItemViewer Instance { get; private set; }

    [SerializeField] private Transform _itemRoot;
    [SerializeField] private Camera _itemViewerCamera;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    [SerializeField] private float _rotateSpeed = 0.5f;
    [SerializeField] private float _zoomSpeed = 1f;
    [SerializeField] private float _minZoom = 2f;
    [SerializeField] private float _maxZoom = 8f;

    private GameObject _currentItem;

    private bool _isDragging;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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

        Vector3 pos = _itemViewerCamera.transform.localPosition;
        pos.z += scrollDelta * _zoomSpeed;
        pos.z = Mathf.Clamp(pos.z, -_maxZoom, -_minZoom);
        _itemViewerCamera.transform.localPosition = pos;
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

    public void ShowItem(ItemData itemData)
    {
        if (_currentItem != null)
            Destroy(_currentItem);

        _currentItem = Instantiate(itemData.Prefab, _itemRoot, false);
        _currentItem.transform.localPosition = Vector3.zero;
        SetLayerRecursively(_currentItem, _itemRoot.gameObject.layer);
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
}