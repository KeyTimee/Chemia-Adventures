using System.Collections.Generic;
using UnityEngine;

public class Inventory_UI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Player player;
    public List<Slot_UI> slots = new List<Slot_UI>();

    // 🔥 (opsional, kalau masih dipakai)
    public ExperimentSystem experimentSystem;

    // ICON
    public Sprite woodIcon;
    public Sprite acidWaterIcon;
    public Sprite limestonePowderIcon;
    public Sprite neutralWaterIcon;
    public Sprite limestoneIcon;

    public bool isMainInventory = false;

    void Start()
    {
        inventoryPanel.SetActive(false);

        // 🔥 AUTO UPDATE UI SAAT DATA BERUBAH
        if (player != null && player.inventory != null)
        {
            player.inventory.onInventoryChanged += Setup;
        }
        else
        {
            Debug.LogError("Player atau Inventory belum di-assign!");
        }
    }

    void Update()
    {
        if (isMainInventory && Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(false);
        }
    }

    public void ToggleInventory()
    {
        if (!inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(true);
            Setup();
        }
        else
        {
            inventoryPanel.SetActive(false);
        }
    }

    public void Setup()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            // 🔥 PASTIKAN SLOT TAHU INDEX & REFERENSI
            slots[i].slotIndex = i;
            slots[i].inventoryUI = this;

            if (i < player.inventory.slots.Count)
            {
                var slot = player.inventory.slots[i];

                if (slot.type != CollectableType.NONE)
                {
                    Sprite icon = GetIcon(slot.type);
                    slots[i].SetItem(icon, slot.count, slot.type);
                }
                else
                {
                    slots[i].Clear();
                }
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    Sprite GetIcon(CollectableType type)
    {
        switch (type)
        {
            case CollectableType.WOOD: return woodIcon;
            case CollectableType.ACID_WATER: return acidWaterIcon;
            case CollectableType.LIMESTONEPOWDER: return limestonePowderIcon;
            case CollectableType.NEUTRAL_WATER: return neutralWaterIcon;
            case CollectableType.LIMESTONE: return limestoneIcon;
        }

        return null;
    }
}