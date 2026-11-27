using UnityEngine;
using TMPro;

public class GrinderSystem : MonoBehaviour
{
    [Header("Slots")]
    public ExperimentSlot_UI inputSlot;   // Slot untuk Batu Kapur (LIMESTONE)
    public ExperimentSlot_UI resultSlot;  // Slot untuk hasil Bubuk Kapur (LIMESTONEPOWDER)

    [Header("UI Feedback (Opsional)")]
    public GameObject statusPanel;
    public TextMeshProUGUI statusText;

    [Header("Icons")]
    public Sprite limestonePowderIcon;    // Sprite hasil grinding

    [Header("References")]
    public Player player;
    public Inventory_UI inventoryUI;

    public void ProcessGrind()
    {
        if (inputSlot == null || resultSlot == null)
        {
            Debug.LogError("[Grinder] Slot UI belum di-assign di Inspector!");
            return;
        }

        Inventory targetInventory = null;
        if (InventoryManager.Instance != null && InventoryManager.Instance.inventory != null)
        {
            targetInventory = InventoryManager.Instance.inventory;
        }
        else if (player != null && player.inventory != null)
        {
            targetInventory = player.inventory;
        }

        if (targetInventory == null)
        {
            Debug.LogError("[Grinder] Inventory tidak ditemukan!");
            return;
        }

        var inputItem = inputSlot.GetItem();

        if (inputItem == CollectableType.NONE)
        {
            ShowStatus("Masukkan item batu kapur (LIMESTONE) terlebih dahulu!");
            return;
        }

        int inputIndex = inputSlot.GetSourceIndex();
        if (inputIndex < 0)
        {
            Debug.LogError("[Grinder] Source index item tidak valid!");
            return;
        }

        if (inputItem == CollectableType.LIMESTONE)
        {
            resultSlot.SetItem(limestonePowderIcon, CollectableType.LIMESTONEPOWDER);
            targetInventory.RemoveAt(inputIndex);
            targetInventory.AddAmount(CollectableType.LIMESTONEPOWDER, 3);

            ShowStatus("Berhasil menghaluskan 1 Bongkah Batu Kapur menjadi 3 takaran Serbuk Kapur (CaCO3)!");
            inputSlot.Clear();

            // Segarkan tampilan Inventory UI sebelah kiri
            if (inventoryUI != null)
                inventoryUI.Setup();
        }
        else
        {
            ShowStatus("Alat penumbuk (Grinder) ini hanya bisa memproses Batu Kapur (LIMESTONE)!");
        }
    }

    public void ClearAllSlots()
    {
        if (inputSlot != null) inputSlot.Clear();
        if (resultSlot != null) resultSlot.Clear();
    }

    public void ShowStatus(string message)
    {
        if (statusPanel != null) statusPanel.SetActive(true);
        if (statusText != null) statusText.text = message;
    }
}