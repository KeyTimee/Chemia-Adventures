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
    public Sprite soilSuspensionIcon;
    public Sprite limedSoilSuspensionIcon; // 🔥 Icon untuk Ekstrak Tanah + Kapur

    [Header("References (Optional)")]
    public Player player;
    public Inventory_UI inventoryUI;

    public void Mix()
    {
        if (sampleSlot == null || materialSlot == null || resultSlot == null)
        {
            Debug.LogError("Slot UI Experiment belum di-assign di Inspector!");
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
            Debug.LogError("Gagal Memproses Mix: Inventory/InventoryManager tidak ditemukan!");
            return;
        }

        var sample = sampleSlot.GetItem();
        var material = materialSlot.GetItem();

        if (sample == CollectableType.NONE || material == CollectableType.NONE)
        {
            ShowReaction("Slot belum lengkap", "Masukkan 2 bahan terlebih dahulu.");
            return;
        }

        int sampleIndex = sampleSlot.GetSourceIndex();
        int materialIndex = materialSlot.GetSourceIndex();

        if (sampleIndex < 0 || materialIndex < 0)
        {
            Debug.LogError("Source index tidak valid!");
            return;
        }

        // TAHAP 1: Sampel Tanah + Air Netral -> Suspensi Tanah
        bool isMakingSoilSuspension = (sample == CollectableType.SOIL_SAMPLE && material == CollectableType.NEUTRAL_WATER) ||
                                      (sample == CollectableType.NEUTRAL_WATER && material == CollectableType.SOIL_SAMPLE);

        // TAHAP 2 (QUEST 1): Suspensi Tanah + Serbuk Kapur -> Limed Soil Suspension
        bool isNeutralizingSoil = (sample == CollectableType.SOIL_SUSPENSION && material == CollectableType.LIMESTONEPOWDER) ||
                                  (sample == CollectableType.LIMESTONEPOWDER && material == CollectableType.SOIL_SUSPENSION);

        // REAKSI LAIN: Air Asam + Serbuk Kapur
        bool isAcidAndLimestone = (sample == CollectableType.ACID_WATER && material == CollectableType.LIMESTONEPOWDER) ||
                                  (sample == CollectableType.LIMESTONEPOWDER && material == CollectableType.ACID_WATER);


        if (isMakingSoilSuspension)
        {
            resultSlot.SetItem(soilSuspensionIcon, CollectableType.SOIL_SUSPENSION);
            KurangiBahan(targetInventory, sampleIndex, materialIndex);
            targetInventory.Add(CollectableType.SOIL_SUSPENSION);

            ShowReaction(
                "Ekstraksi Sampel Tanah (Soil Extraction)",
                "Sampel tanah dilarutkan dalam air murni untuk melarutkan ion H+ agar reaksi kimia dapat mengukur/menetralkan keasamannya."
            );
        }
        else if (isNeutralizingSoil)
        {
            // 🔥 A. Set ikon hasil ke Limed Soil Suspension di slot result
            resultSlot.SetItem(limedSoilSuspensionIcon, CollectableType.LIMED_SOIL_SUSPENSION);

            // B. Kurangi bahan penyusun dari Inventory
            KurangiBahan(targetInventory, sampleIndex, materialIndex);

            // 🔥 C. Masukkan item LIMED_SOIL_SUSPENSION ke Inventory
            targetInventory.Add(CollectableType.LIMED_SOIL_SUSPENSION);

            // D. Advansikan status Quest
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.AdvanceQuest1(MainQuestState.ExperimentDone);
            }

            ShowReaction(
                "CaCO<sub>3</sub> + H<sup>+</sup> (Ekstrak Tanah Asam) → Netralisasi",
                "Analisis Berhasil! Larutan kapur (CaCO3) berhasil menetralkan keasaman pada ekstrak tanah. Sekarang kamu bisa membawa hasil ini untuk disiramkan/ditaburkan ke tanah perkebunan!"
            );
        }
        else if (isAcidAndLimestone)
        {
            resultSlot.SetItem(neutralWaterIcon, CollectableType.NEUTRAL_WATER);
            KurangiBahan(targetInventory, sampleIndex, materialIndex);
            targetInventory.Add(CollectableType.NEUTRAL_WATER);

            ShowReaction(
                "CaCO<sub>3</sub> + 2H<sup>+</sup> → Ca<sup>2+</sup> + CO<sub>2</sub> + H<sub>2</sub>O",
                "Kapur (CaCO3) menetralkan air asam dan menghasilkan gas CO2 serta air netral."
            );
        }
        else
        {
            resultSlot.Clear();
            ShowReaction("Reaksi gagal", "Kombinasi bahan kimia tidak cocok.");
        }

        sampleSlot.Clear();
        materialSlot.Clear();
    }

    private void KurangiBahan(Inventory inv, int indexA, int indexB)
    {
        if (indexA == indexB)
        {
            inv.RemoveAt(indexA);
            inv.RemoveAt(indexA);
        }
        else
        {
            int firstToRemove = Mathf.Max(indexA, indexB);
            int secondToRemove = Mathf.Min(indexA, indexB);

            inv.RemoveAt(firstToRemove);
            inv.RemoveAt(secondToRemove);
        }
    }

    public void ShowReaction(string reaction, string description)
    {
        if (reactionPanel != null)
            reactionPanel.SetActive(true);

        if (reactionText != null)
            reactionText.text = reaction;

        if (descriptionText != null)
            descriptionText.text = description;
    }
}