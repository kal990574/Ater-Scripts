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

    private Vector3 _initialCameraLocalPosition;
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
        if (_itemViewerCamera == null)
        {
            return;
        }

        float nextFieldOfView = _itemViewerCamera.fieldOfView - (scrollDelta * _zoomSpeed);
        _itemViewerCamera.fieldOfView = Mathf.Clamp(nextFieldOfView, _minFieldOfView, _maxFieldOfView);
    }

    public void ShowItem(string instanceId)
    {
        _itemRoot.rotation = Quaternion.identity;
        _itemViewerCamera.transform.localPosition = _initialCameraLocalPosition;
        _itemViewerCamera.fieldOfView = _maxFieldOfView;

        ItemInstanceData itemInstanceData = InventoryManager.Instance != null
            ? InventoryManager.Instance.GetItemInstance(instanceId)
            : null;

        InventoryManager.Instance.ShowExamineItem(instanceId);
        _descriptionText.text = itemInstanceData != null ? itemInstanceData.Description : string.Empty;
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
            ExamineInteractPoint examineInteractPoint = hit.collider.GetComponentInParent<ExamineInteractPoint>();
            if (examineInteractPoint != null)
            {
                examineInteractPoint?.OnClick();
            }
        }
    }

    public void Hide()
    {
        InventoryManager.Instance.HideExamineItem();
        _descriptionText.text = string.Empty;
    }

    private void HandleRotate()
    {
        if (!_isDragging)
        {
            return;
        }

        Vector2 delta = Mouse.current.delta.ReadValue();
        _itemRoot.Rotate(Vector3.up, -delta.x * _rotateSpeed, Space.World);
        _itemRoot.Rotate(Vector3.right, delta.y * _rotateSpeed, Space.World);
    }

    private void OnDisable()
    {
        _isDragging = false;
        InventoryManager.Instance.HideExamineItem();
    }
}
