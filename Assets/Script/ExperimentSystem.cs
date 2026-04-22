using UnityEngine;
using TMPro;

public class ExperimentSystem : MonoBehaviour
{
    [Header("Slot")]
    public ExperimentSlot_UI sampleSlot;
    public ExperimentSlot_UI materialSlot;
    public ExperimentSlot_UI resultSlot;

    [Header("UI Reaction")]
    public GameObject reactionPanel;
    public TextMeshProUGUI reactionText;
    public TextMeshProUGUI descriptionText;

    [Header("Icons")]
    public Sprite acidWaterIcon;
    public Sprite limestonePowderIcon;
    public Sprite neutralWaterIcon;

    [Header("References")]
    public Player player;
    public Inventory_UI inventoryUI;

    public void Mix()
    {
        // 🔥 SAFETY CHECK
        if (sampleSlot == null || materialSlot == null || resultSlot == null)
        {
            Debug.LogError("Slot belum di-assign!");
            return;
        }

        if (player == null || player.inventory == null)
        {
            Debug.LogError("Player atau Inventory belum di-assign!");
            return;
        }

        var sample = sampleSlot.GetItem();
        var material = materialSlot.GetItem();

        // ❗ VALIDASI SLOT KOSONG
        if (sample == CollectableType.NONE || material == CollectableType.NONE)
        {
            ShowReaction("Slot belum lengkap", "Masukkan 2 bahan terlebih dahulu.");
            return;
        }

        // 🔥 AMBIL INDEX ASAL DARI INVENTORY
        int sampleIndex = sampleSlot.GetSourceIndex();
        int materialIndex = materialSlot.GetSourceIndex();

        // 🔥 VALIDASI INDEX
        if (sampleIndex < 0 || materialIndex < 0)
        {
            Debug.LogError("Source index tidak valid!");
            return;
        }

        // =========================
        // 🔥 KURANGI ITEM (CONSUME)
        // =========================
        if (sampleIndex == materialIndex)
        {
            // ambil 2 dari slot yang sama
            player.inventory.RemoveAt(sampleIndex);
            player.inventory.RemoveAt(sampleIndex);
        }
        else
        {
            player.inventory.RemoveAt(sampleIndex);
            player.inventory.RemoveAt(materialIndex);
        }

        // =========================
        // 🔥 CEK REAKSI
        // =========================
        if (sample == CollectableType.ACID_WATER && material == CollectableType.LIMESTONEPOWDER)
        {
            resultSlot.SetItem(neutralWaterIcon, CollectableType.NEUTRAL_WATER);

            // 🔥 TAMBAH HASIL KE INVENTORY
            player.inventory.Add(CollectableType.NEUTRAL_WATER);

            ShowReaction(
                "CaCO<sub>3</sub> + 2H<sup>+</sup> → Ca<sup>2+</sup> + CO<sub>2</sub> + H<sub>2</sub>O",
                "Kapur menetralkan air asam dan menghasilkan gas CO2."
            );
        }
        else
        {
            resultSlot.Clear();
            ShowReaction("Reaksi gagal", "Kombinasi tidak cocok.");
        }

        // 🔥 CLEAR SLOT EXPERIMENT
        sampleSlot.Clear();
        materialSlot.Clear();
    }

    void ShowReaction(string reaction, string description)
    {
        if (reactionPanel != null)
            reactionPanel.SetActive(true);

        if (reactionText != null)
            reactionText.text = reaction;

        if (descriptionText != null)
            descriptionText.text = description;
    }
}