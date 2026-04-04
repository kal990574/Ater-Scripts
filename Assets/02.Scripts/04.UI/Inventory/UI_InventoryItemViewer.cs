using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_InventoryItemViewer : MonoBehaviour
{
    [SerializeField] private Transform _itemRoot;
    [SerializeField] private Camera _itemViewerCamera;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Transform _examineRoot;

    [SerializeField] private float _rotateSpeed = 0.5f;
    [SerializeField] private float _zoomSpeed = 1.0f;
    [SerializeField] private float _minFieldOfView = 20.0f;
    [SerializeField] private float _maxFieldOfView = 60.0f;

    private Vector3 _initialCameraLocalPosition;
    private bool _isDragging;

    private RuntimeInstanceManager _runtimeInstanceManager;
    private ExamineViewService _examineViewService;
    private RuntimeItemFactory runtimeItemFactory;

    private void Start()
    {
        _initialCameraLocalPosition = _itemViewerCamera.transform.localPosition;
        _itemViewerCamera.transform.LookAt(_itemRoot);

        _runtimeInstanceManager = RuntimeInstanceManager.Instance;
        if (_runtimeInstanceManager != null)
        {
            runtimeItemFactory = new RuntimeItemFactory(_runtimeInstanceManager);
            _examineViewService = new ExamineViewService(runtimeItemFactory, _examineRoot);
        }
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

        if (_runtimeInstanceManager == null || _examineViewService == null)
        {
            return;
        }

        RuntimeItemData runtimeItemData = _runtimeInstanceManager.GetItemInstance(instanceId);
        if (runtimeItemData == null)
        {
            _descriptionText.text = string.Empty;
            _examineViewService.Hide();
            return;
        }

        _examineViewService.Show(runtimeItemData);
        _descriptionText.text = runtimeItemData.Description;
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
                examineInteractPoint.OnClick();
            }
        }
    }

    public void Hide()
    {
        _examineViewService?.Hide();
        _descriptionText.text = string.Empty;
    }

    private void HandleRotate()
    {
        if (_isDragging == false)
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
        _examineViewService?.Hide();
    }
}