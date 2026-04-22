using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ExperimentSlot_UI : MonoBehaviour, IDropHandler, IPointerEnterHandler
{
    public Image icon;

    private CollectableType currentItem;

    // 🔥 TAMBAHAN
    private int sourceIndex = -1;

    public ExperimentSystem experimentSystem;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("DROP MASUK");

        if (currentItem != CollectableType.NONE) return;

        Slot_UI draggedSlot = eventData.pointerDrag.GetComponent<Slot_UI>();

        if (draggedSlot != null)
        {
            CollectableType type = draggedSlot.GetItemType();
            Sprite sprite = draggedSlot.icon.sprite;

            // 🔥 SIMPAN JUGA INDEX ASAL
            SetItem(sprite, type, draggedSlot.slotIndex);
        }
    }

    // 🔥 UPDATE METHOD
    public void SetItem(Sprite sprite, CollectableType type, int index)
    {
        icon.sprite = sprite;
        icon.enabled = true;
        currentItem = type;
        sourceIndex = index;
    }

    public void SetItem(Sprite sprite, CollectableType type)
    {
        SetItem(sprite, type, -1); // ✔ BENAR
    }

    public CollectableType GetItem()
    {
        return currentItem;
    }

    // 🔥 TAMBAHAN
    public int GetSourceIndex()
    {
        return sourceIndex;
    }

    public void Clear()
    {
        icon.sprite = null;
        icon.enabled = false;
        currentItem = CollectableType.NONE;
        sourceIndex = -1;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("MOUSE MASUK SLOT");
    }
}