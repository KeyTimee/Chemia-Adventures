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

    public void ProcessGrind()
    {
        // 1. Safety Check Slot UI
        if (inputSlot == null || resultSlot == null)
        {
            Debug.LogError("[Grinder] Slot UI belum di-assign di Inspector!");
            return;
        }

        // 2. Ambil Inventory Manager
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

        // 3. Ambil Item dari Input Slot
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

        // 4. Cek Pemrosesan Grinding (LIMESTONE -> LIMESTONEPOWDER)
        if (inputItem == CollectableType.LIMESTONE)
        {
            // A. Set hasil di Result Slot
            resultSlot.SetItem(limestonePowderIcon, CollectableType.LIMESTONEPOWDER);

            // B. Kurangi 1 Batu Kapur dari Inventory Utama
            targetInventory.RemoveAt(inputIndex);

            // C. Tambahkan 1 Bubuk Kapur ke Inventory Utama
            targetInventory.Add(CollectableType.LIMESTONEPOWDER);

            // D. Tampilkan Status Penjelasan
            ShowStatus("Berhasil menghancurkan Batu Kapur (CaCO3) menjadi Serbuk Kapur halus!");

            // E. Bersihkan Input Slot setelah digrind
            inputSlot.Clear();
        }
        else
        {
            ShowStatus("Alat penumbuk (Grinder) ini hanya bisa memproses Batu Kapur (LIMESTONE)!");
        }
    }

    public void ShowStatus(string message)
    {
        if (statusPanel != null) statusPanel.SetActive(true);
        if (statusText != null) statusText.text = message;
    }
}