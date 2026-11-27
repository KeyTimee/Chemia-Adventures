using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ExperimentSlot_UI : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerClickHandler
{
    public Image icon;
    public bool isResultSlot = false; // Centang HANYA pada slot Result!

    private CollectableType currentItem;
    private int sourceIndex = -1;

    public ExperimentSystem experimentSystem;

    private void Awake()
    {
        Clear();
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Cegah pemain men-drop item ke slot Result
        if (isResultSlot) return;

        Slot_UI draggedSlot = eventData.pointerDrag != null ? eventData.pointerDrag.GetComponent<Slot_UI>() : null;

        if (draggedSlot != null && draggedSlot.GetItemType() != CollectableType.NONE)
        {
            CollectableType type = draggedSlot.GetItemType();
            Sprite sprite = draggedSlot.icon.sprite;

            SetItem(sprite, type, draggedSlot.slotIndex);
        }
    }

    // Klik pada slot input untuk mengosongkannya jika salah taruh item
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isResultSlot && currentItem != CollectableType.NONE)
        {
            Clear();
        }
    }

    public void SetItem(Sprite sprite, CollectableType type, int index)
    {
        icon.sprite = sprite;
        icon.enabled = true;
        currentItem = type;
        sourceIndex = index;
    }

    public void SetItem(Sprite sprite, CollectableType type)
    {
        SetItem(sprite, type, -1);
    }

    public CollectableType GetItem()
    {
        return currentItem;
    }

    public int GetSourceIndex()
    {
        return sourceIndex;
    }

    public void Clear()
    {
        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }
        currentItem = CollectableType.NONE;
        sourceIndex = -1;
    }

    public void OnPointerEnter(PointerEventData eventData) { }
}