using System.Collections.Generic;
using UnityEngine;

public class Inventory_UI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public List<Slot_UI> slots = new List<Slot_UI>();

    public ExperimentSystem experimentSystem;

    [Header("Item Icons")]
    public Sprite woodIcon;
    public Sprite acidWaterIcon;
    public Sprite limestonePowderIcon;
    public Sprite neutralWaterIcon;
    public Sprite limestoneIcon;
    public Sprite soilSampleIcon;
    public Sprite soilSuspensionIcon;
    public Sprite limedSoilSuspensionIcon; // 🔥 1. Variabel Icon Baru
    public Sprite wateringCanIcon;

    [Header("Industrial S.T.E Icons")]
    public Sprite seawaterIcon;
    public Sprite naohIcon;
    public Sprite h2so4Icon;
    public Sprite mgoh2Icon;

    [Header("Main Quest 2 Icons")]
    public Sprite gasDetectorIcon;
    public Sprite manureSampleIcon;
    public Sprite concentratedVinegarIcon;
    public Sprite diluteAceticAcidIcon;
    public Sprite ammoniaExtractIcon;
    public Sprite ammoniumAcetateIcon;

    [Header("UI Settings")]
    public bool isMainInventory = false;

    private Inventory inventory;

    private void OnEnable()
    {
        HubungkanKeInventoryManager();
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.onInventoryChanged -= Setup;
        }
    }

    void Start()
    {
        if (inventoryPanel != null && isMainInventory)
        {
            inventoryPanel.SetActive(false);
        }

        HubungkanKeInventoryManager();
    }

    private void HubungkanKeInventoryManager()
    {
        if (InventoryManager.Instance != null && InventoryManager.Instance.inventory != null)
        {
            if (inventory != null)
            {
                inventory.onInventoryChanged -= Setup;
            }

            inventory = InventoryManager.Instance.inventory;
            inventory.onInventoryChanged += Setup;

            Setup();
        }
    }

    void Update()
    {
        if (isMainInventory && Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && inventoryPanel != null && inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(false);
        }
    }

    public void ToggleInventory()
    {
        if (inventoryPanel == null) return;

        bool statusSekarang = inventoryPanel.activeSelf;
        inventoryPanel.SetActive(!statusSekarang);

        if (!statusSekarang)
        {
            Setup();
        }
    }

    public void Setup()
    {
        if (inventory == null)
        {
            HubungkanKeInventoryManager();
            if (inventory == null) return;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].slotIndex = i;
            slots[i].inventoryUI = this;

            if (i < inventory.slots.Count)
            {
                var slot = inventory.slots[i];

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
        Sprite result = null;

        switch (type)
        {
            case CollectableType.WOOD: result = woodIcon; break;
            case CollectableType.ACID_WATER: result = acidWaterIcon; break;
            case CollectableType.LIMESTONEPOWDER: result = limestonePowderIcon; break;
            case CollectableType.NEUTRAL_WATER: result = neutralWaterIcon; break;
            case CollectableType.LIMESTONE: result = limestoneIcon; break;
            case CollectableType.SOIL_SAMPLE: result = soilSampleIcon; break;
            case CollectableType.SOIL_SUSPENSION: result = soilSuspensionIcon; break;
            case CollectableType.LIMED_SOIL_SUSPENSION: result = limedSoilSuspensionIcon; break;
            case CollectableType.WATERING_CAN: result = wateringCanIcon; break;
            case CollectableType.SEAWATER: result = seawaterIcon; break;
            case CollectableType.NAOH: result = naohIcon; break;
            case CollectableType.H2SO4: result = h2so4Icon; break;
            case CollectableType.MGOH2: result = mgoh2Icon; break;

            // --- Main Quest 2 Icons ---
            case CollectableType.GAS_DETECTOR: result = gasDetectorIcon; break;
            case CollectableType.MANURE_SAMPLE: result = manureSampleIcon; break;
            case CollectableType.CONCENTRATED_VINEGAR: result = concentratedVinegarIcon; break;
            case CollectableType.DILUTE_ACETIC_ACID: result = diluteAceticAcidIcon; break;
            case CollectableType.AMMONIA_EXTRACT: result = ammoniaExtractIcon; break;
            case CollectableType.AMMONIUM_ACETATE: result = ammoniumAcetateIcon; break;

            default:
                Debug.LogWarning("GetIcon: Tipe Item " + type + " BELUM DIDAFTARKAN di Switch-Case!");
                return null;
        }

        if (result == null)
        {
            Debug.LogWarning("GetIcon: Tipe Item " + type + " DITEMUKAN, tetapi Slot Sprite-nya di Inspector MASIH KOSONG (NONE)!");
        }

        return result;
    }
}