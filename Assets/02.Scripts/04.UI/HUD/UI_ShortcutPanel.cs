using UnityEngine;
using UnityEngine.UI;

public class UI_ShortcutPanel : MonoBehaviour
{
    [SerializeField] private float _normalAlpha = 0.4f;
    [SerializeField] private float _selectedAlpha = 1.0f;

    private Image[] _iconImages;
    private CanvasGroup[] _canvasGroup;

    private void Awake()
    {
        int count = transform.childCount;
        _iconImages = new Image[count];
        _canvasGroup = new CanvasGroup[count];

        for (int i = 0; i < count; i++)
        {
            Transform slot = transform.GetChild(i);
            Transform iconChild = slot.Find("Icon");
            _iconImages[i] = iconChild != null ? iconChild.GetComponent<Image>() : null;

            _canvasGroup[i] = slot.GetComponent<CanvasGroup>();
            if (_canvasGroup[i] == null)
            {
                _canvasGroup[i] = slot.gameObject.AddComponent<CanvasGroup>();
            }
        }
    }

    private void Start()
    {
        Refresh();
    }

    private void OnEnable()
    {
        if (InventoryManager.Instance == null) return;

        InventoryManager.Instance.OnInventoryItemChanged += Refresh;
        InventoryManager.Instance.OnHandSlotChanged += UpdateHighlight;
        Refresh();
    }

    private void OnDisable()
    {
        if(InventoryManager.Instance == null) return;
        InventoryManager.Instance.OnInventoryItemChanged -= Refresh;
        InventoryManager.Instance.OnHandSlotChanged -= UpdateHighlight;
    }

    private void Refresh()
    {
        for(int i = 0; i < _iconImages.Length; i++)
        {
            string instanceId = InventoryManager.Instance.GetInventoryItemInstanceIdAt(i);
            RuntimeItemData data = string.IsNullOrEmpty(instanceId) ? null : InventoryManager.Instance.GetItemInstance(instanceId);

            _iconImages[i].sprite = data?.Icon;
            _iconImages[i].enabled = data != null;
        }
    }
    private void UpdateHighlight(int handIndex)
    {
        for (int i = 0; i < _canvasGroup.Length; i++)
        {
            _canvasGroup[i].alpha = i == handIndex ? _selectedAlpha : _normalAlpha;
        }
    }
}
