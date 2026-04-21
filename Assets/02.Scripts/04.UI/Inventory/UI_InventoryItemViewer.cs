using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_InventoryItemViewer : MonoBehaviour
{
    [SerializeField] private Transform _examineRoot;
    [SerializeField] private Camera _itemViewerCamera;

    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    [SerializeField] private float _rotateSpeed = 0.5f;
    [SerializeField] private float _moveSpeed = 0.0025f;
    [SerializeField] private float _maxMoveDistance = 0.5f;
    [SerializeField] private float _zoomSpeed = 1.0f;
    [SerializeField] private float _minFieldOfView = 20.0f;
    [SerializeField] private float _maxFieldOfView = 60.0f;

    private Vector3 _initialCameraLocalPosition;
    private Vector3 _initialRootLocalPosition;
    private bool _isDragging;
    private bool _isMoveDragging;

    private RuntimeInstanceManager _runtimeInstanceManager;
    private ExamineViewService _examineViewService;
    private RuntimeItemFactory runtimeItemFactory;

    private void Start()
    {
        _initialCameraLocalPosition = _itemViewerCamera.transform.localPosition;
        _initialRootLocalPosition = _examineRoot.localPosition;
        _itemViewerCamera.transform.LookAt(_examineRoot);

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
        HandleMove();
    }

    public void SetDragging(bool isDragging)
    {
        _isDragging = isDragging;
    }

    public void SetMoveDragging(bool isMoveDragging)
    {
        _isMoveDragging = isMoveDragging;
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
        _examineRoot.rotation = Quaternion.identity;
        _examineRoot.localPosition = _initialRootLocalPosition;
        _itemViewerCamera.transform.localPosition = _initialCameraLocalPosition;
        _itemViewerCamera.fieldOfView = _maxFieldOfView;

        if (_runtimeInstanceManager == null || _examineViewService == null)
        {
            return;
        }

        RuntimeItemData runtimeItemData = _runtimeInstanceManager.GetItemInstance(instanceId);
        if (runtimeItemData == null)
        {
            _itemNameText.text = string.Empty;
            _descriptionText.text = string.Empty;
            _examineViewService.Hide();
            return;
        }

        _examineViewService.Show(runtimeItemData);
        _itemNameText.text = runtimeItemData.ItemName;
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
        _itemNameText.text = string.Empty;
        _descriptionText.text = string.Empty;
    }

    private void HandleRotate()
    {
        if (_isDragging == false || Mouse.current == null)
        {
            return;
        }

        Vector2 delta = Mouse.current.delta.ReadValue();
        _examineRoot.Rotate(Vector3.up, -delta.x * _rotateSpeed, Space.World);
        _examineRoot.Rotate(Vector3.right, delta.y * _rotateSpeed, Space.World);
    }

    private void HandleMove()
    {
        if (_isMoveDragging == false || Mouse.current == null)
        {
            return;
        }

        Vector2 delta = Mouse.current.delta.ReadValue();
        if (delta.sqrMagnitude <= 0.0f)
        {
            return;
        }

        Vector3 worldOffset =
            (_itemViewerCamera.transform.right * delta.x + _itemViewerCamera.transform.up * delta.y) * _moveSpeed;

        Vector3 localOffset = _examineRoot.parent == null
            ? worldOffset
            : _examineRoot.parent.InverseTransformVector(worldOffset);

        Vector3 targetLocalPosition = _examineRoot.localPosition + localOffset;
        Vector3 fromInitial = targetLocalPosition - _initialRootLocalPosition;
        _examineRoot.localPosition = _initialRootLocalPosition + Vector3.ClampMagnitude(fromInitial, _maxMoveDistance);
    }

    private void OnDisable()
    {
        _isDragging = false;
        _isMoveDragging = false;
        _examineViewService?.Hide();
    }
}
