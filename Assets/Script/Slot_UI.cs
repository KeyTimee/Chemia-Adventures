using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Slot_UI : MonoBehaviour, 
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image icon;
    public TextMeshProUGUI countText;

    // 🔥 DATA LINK
    public int slotIndex;
    public Inventory_UI inventoryUI;

    private CanvasGroup canvasGroup;
    private CollectableType itemType;

    public System.Action<CollectableType> onItemClicked;

    // 🔥 DRAG ICON (assign dari Inspector)
    public Image dragIcon;

    public void SetItem(Sprite sprite, int count, CollectableType type)
    {
        icon.sprite = sprite;
        icon.color = Color.white;
        countText.text = count.ToString();
        itemType = type;
    }

    public void Clear()
    {
        icon.sprite = null;
        icon.color = new Color(1, 1, 1, 0);
        countText.text = "";
        itemType = CollectableType.NONE;
    }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (dragIcon == null)
        {
            Debug.LogError("DragIcon belum di-assign di Inspector!");
        }
    }

    // ======================
    // 🔥 DRAG SYSTEM
    // ======================

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemType == CollectableType.NONE) return;
        if (dragIcon == null) return;

        dragIcon.sprite = icon.sprite;
        dragIcon.color = new Color(1, 1, 1, 0.8f);
        dragIcon.gameObject.SetActive(true);

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon == null) return;

        dragIcon.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            dragIcon.gameObject.SetActive(false);
        }

        canvasGroup.blocksRaycasts = true;
    }

    // ======================
    // 🔥 DROP (FULL SYNC)
    // ======================

    public void OnDrop(PointerEventData eventData)
    {
        Slot_UI draggedSlot = eventData.pointerDrag.GetComponent<Slot_UI>();

        if (draggedSlot == null) return;
        if (draggedSlot == this) return;

        // 🔥 VALIDASI
        if (inventoryUI == null || inventoryUI.player == null) return;

        // 🔥 SWAP DATA (INI KUNCI)
        inventoryUI.player.inventory.Swap(draggedSlot.slotIndex, this.slotIndex);
    }

    public CollectableType GetItemType()
    {
        return itemType;
    }
}