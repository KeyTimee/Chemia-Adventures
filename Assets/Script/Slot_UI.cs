using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Slot_UI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image icon;
    public TextMeshProUGUI countText;

    // 🔥 DATA LINK (Otomatis terisi lewat code)
    public int slotIndex;
    [HideInInspector] public Inventory_UI inventoryUI;

    private CanvasGroup canvasGroup;
    private CollectableType itemType;

    public System.Action<CollectableType> onItemClicked;

    // 🔥 DRAG ICON
    [HideInInspector] public Image dragIcon;

    public void SetItem(Sprite sprite, int count, CollectableType type)
    {
        icon.sprite = sprite;
        icon.color = Color.white;
        itemType = type;

        // 🔥 TAMPILKAN ANGKA HANYA JIKA ITEM LEBIH DARI 1 (STACKING)
        if (countText != null)
        {
            if (count > 1)
            {
                countText.text = count.ToString();
                countText.gameObject.SetActive(true);
            }
            else
            {
                countText.text = "";
                countText.gameObject.SetActive(false); // Sembunyikan jika cuma 1 item
            }
        }
    }

    public void Clear()
    {
        icon.sprite = null;
        icon.color = new Color(1, 1, 1, 0);
        itemType = CollectableType.NONE;

        if (countText != null)
        {
            countText.text = "";
            countText.gameObject.SetActive(false);
        }
    }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // 1. Otomatis cari Inventory_UI di Parent
        if (inventoryUI == null)
        {
            inventoryUI = GetComponentInParent<Inventory_UI>();
        }

        // 2. Cari DragIcon di scene saat Awake
        if (dragIcon == null)
        {
            CariDragIconInScene();
        }
    }

    private void CariDragIconInScene()
    {
        // 1. Ambil dari DragIconManager (Paling cepat & akurat)
        if (DragIconManager.InstanceImage != null)
        {
            dragIcon = DragIconManager.InstanceImage;
            return;
        }

        // 2. Opsi cadangan pencarian biasa di Scene
        GameObject dragObj = GameObject.FindWithTag("DragIcon");
        if (dragObj == null) dragObj = GameObject.Find("DragIcon");

        if (dragObj != null)
        {
            dragIcon = dragObj.GetComponent<Image>();
        }
    }

    // ======================
    // 🔥 DRAG SYSTEM
    // ======================

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("1. OnBeginDrag Terpanggil pada Slot Index: " + slotIndex + " | Item Type: " + itemType);

        // 🛑 PENGAMAN 1: Cegah drag jika slot KOSONG
        if (itemType == CollectableType.NONE)
        {
            Debug.LogWarning("Drag Dibatalkan: Slot ini KOSONG (NONE)");
            return;
        }

        // 🛑 PENGAMAN 2: Cegah drag jika Sprite KOSONG
        if (icon == null || icon.sprite == null)
        {
            Debug.LogWarning("Drag Dibatalkan: Gambar Sprite pada slot ini KOSONG/NULL!");
            return;
        }

        // 🛑 PENGAMAN 3: Paksa cari ulang DragIcon jika saat Awake sempat NULL (Efek Cross-Scene Base)
        if (dragIcon == null)
        {
            CariDragIconInScene();
        }

        if (dragIcon == null)
        {
            Debug.LogError("Drag Dibatalkan: Variable 'dragIcon' masih NULL! Pastikan DragIconManager terpasang pada GameObject DragIcon.");
            return;
        }

        Debug.Log("2. DragIcon Ditemukan: " + dragIcon.gameObject.name + " | Mengaktifkan DragIcon...");

        // 🔥 1. Pindahkan DragIcon ke Canvas tempat Slot ini berada agar selalu tampil paling depan
        Canvas currentCanvas = GetComponentInParent<Canvas>();
        if (currentCanvas != null)
        {
            dragIcon.transform.SetParent(currentCanvas.transform, true);
        }

        dragIcon.transform.SetAsLastSibling();

        // 🔥 2. Atur Properti Visual DragIcon
        dragIcon.sprite = icon.sprite;
        dragIcon.color = Color.white;
        dragIcon.preserveAspect = true;
        dragIcon.raycastTarget = false; // Matikan raycastTarget agar mouse dapat menembus ke slot tujuan
        dragIcon.gameObject.SetActive(true);

        // 🔥 3. Paksa Ulang Ukuran & Skala RectTransform Tepat Setelah Sprite Terpasang
        RectTransform rt = dragIcon.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.sizeDelta = new Vector2(80, 80); // Ukuran ikon melayang saat di-drag
            rt.localScale = Vector3.one;        // Skala dipastikan (1, 1, 1)
        }

        // 🔥 4. Tempelkan posisi ke kursor mouse
        dragIcon.transform.position = Input.mousePosition;

        // 🔥 5. Matikan raycast pada slot yang sedang di-drag
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon == null || !dragIcon.gameObject.activeSelf) return;

        // Ikuti posisi kursor mouse
        dragIcon.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            dragIcon.gameObject.SetActive(false);
        }

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }
    }

    // ======================
    // 🔥 DROP (FULL SYNC)
    // ======================

    public void OnDrop(PointerEventData eventData)
    {
        Slot_UI draggedSlot = eventData.pointerDrag != null ? eventData.pointerDrag.GetComponent<Slot_UI>() : null;

        if (draggedSlot == null) return;
        if (draggedSlot == this) return;

        if (InventoryManager.Instance == null) return;

        InventoryManager.Instance.inventory.Swap(
            draggedSlot.slotIndex,
            this.slotIndex
        );
    }

    public CollectableType GetItemType()
    {
        return itemType;
    }
}