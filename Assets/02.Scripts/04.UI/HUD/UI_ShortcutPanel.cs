using _02.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShortcutPanel : MonoBehaviour
{
    [SerializeField] private float _normalAlpha = 0.4f;
    [SerializeField] private float _selectedAlpha = 1.0f;
    [SerializeField] private PlayerHandAbility playerHandAbility;

    private Image[] _iconImages;
    private CanvasGroup[] _canvasGroup;

    private void Awake()
    {
        int count = transform.childCount;
        _iconImages = new Image[count];
        _canvasGroup = new CanvasGroup[count];

        for (int index = 0; index < count; index++)
        {
            Transform slot = transform.GetChild(index);
            Transform iconChild = slot.Find("Icon");
            _iconImages[index] = iconChild != null ? iconChild.GetComponent<Image>() : null;

            _canvasGroup[index] = slot.GetComponent<CanvasGroup>();
            if (_canvasGroup[index] == null)
            {
                _canvasGroup[index] = slot.gameObject.AddComponent<CanvasGroup>();
            }
        }

        if (playerHandAbility == null)
        {
            playerHandAbility = FindFirstObjectByType<PlayerHandAbility>();
        }
    }

    private void OnEnable()
    {
        InventoryManager inventoryManager = InventoryManager.Instance;
        if (inventoryManager != null)
        {
            inventoryManager.OnInventoryItemChanged += Refresh;
        }

        if (playerHandAbility != null)
        {
            playerHandAbility.OnHandSlotChanged += UpdateHighlight;
        }

        Refresh();

        if (playerHandAbility != null)
        {
            UpdateHighlight(playerHandAbility.CurrentHandIndex);
        }
        else
        {
            UpdateHighlight(-1);
        }
    }

    private void OnDisable()
    {
        InventoryManager inventoryManager = InventoryManager.Instance;
        if (inventoryManager != null)
        {
            inventoryManager.OnInventoryItemChanged -= Refresh;
        }

        if (playerHandAbility != null)
        {
            playerHandAbility.OnHandSlotChanged -= UpdateHighlight;
        }
    }

    private void Refresh()
    {
        InventoryManager inventoryManager = InventoryManager.Instance;
        RuntimeInstanceManager runtimeInstanceManager = RuntimeInstanceManager.Instance;

        if (inventoryManager == null || runtimeInstanceManager == null)
        {
            ClearIcons();
            return;
        }

        for (int index = 0; index < _iconImages.Length; index++)
        {
            Image iconImage = _iconImages[index];
            if (iconImage == null)
            {
                continue;
            }

            string instanceId = inventoryManager.GetInventoryItemInstanceIdAt(index);
            RuntimeItemData runtimeItemData = string.IsNullOrEmpty(instanceId)
                ? null
                : runtimeInstanceManager.GetItemInstance(instanceId);

            iconImage.sprite = runtimeItemData != null ? runtimeItemData.Icon : null;
            iconImage.enabled = runtimeItemData != null;
        }
    }

    private void UpdateHighlight(int handIndex)
    {
        for (int index = 0; index < _canvasGroup.Length; index++)
        {
            _canvasGroup[index].alpha = index == handIndex ? _selectedAlpha : _normalAlpha;
        }
    }

    private void ClearIcons()
    {
        for (int index = 0; index < _iconImages.Length; index++)
        {
            Image iconImage = _iconImages[index];
            if (iconImage == null)
            {
                continue;
            }

            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }
}