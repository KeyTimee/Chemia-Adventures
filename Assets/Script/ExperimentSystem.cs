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

    [Header("Icons (Main Quest 1)")]
    public Sprite acidWaterIcon;
    public Sprite limestonePowderIcon;
    public Sprite neutralWaterIcon;
    public Sprite soilSuspensionIcon;
    public Sprite limedSoilSuspensionIcon;

    [Header("Icons (Main Quest 2)")]
    public Sprite diluteAceticAcidIcon;
    public Sprite ammoniaExtractIcon;
    public Sprite ammoniumAcetateIcon;

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

        // ==========================================
        // KOMBINASI REAKSI MAIN QUEST 1
        // ==========================================
        bool isMakingSoilSuspension = (sample == CollectableType.SOIL_SAMPLE && material == CollectableType.NEUTRAL_WATER) ||
                                      (sample == CollectableType.NEUTRAL_WATER && material == CollectableType.SOIL_SAMPLE);

        bool isNeutralizingSoil = (sample == CollectableType.SOIL_SUSPENSION && material == CollectableType.LIMESTONEPOWDER) ||
                                  (sample == CollectableType.LIMESTONEPOWDER && material == CollectableType.SOIL_SUSPENSION);

        bool isAcidAndLimestone = (sample == CollectableType.ACID_WATER && material == CollectableType.LIMESTONEPOWDER) ||
                                  (sample == CollectableType.LIMESTONEPOWDER && material == CollectableType.ACID_WATER);

        // ==========================================
        // KOMBINASI REAKSI MAIN QUEST 2
        // ==========================================
        // 1. Pengenceran Biang Cuka -> 3 unit Asam Asetat Encer (1 untuk uji lab, 2 untuk semprot kandang)
        bool isDilutingVinegar = (sample == CollectableType.CONCENTRATED_VINEGAR && material == CollectableType.NEUTRAL_WATER) ||
                                 (sample == CollectableType.NEUTRAL_WATER && material == CollectableType.CONCENTRATED_VINEGAR);

        // 2. Ekstraksi Sampel Kotoran Sapi -> Ekstrak Amonia (NH3 aq)
        bool isExtractingAmmonia = (sample == CollectableType.MANURE_SAMPLE && material == CollectableType.NEUTRAL_WATER) ||
                                   (sample == CollectableType.NEUTRAL_WATER && material == CollectableType.MANURE_SAMPLE);

        // 3. Netralisasi Ekstrak Amonia + Asam Asetat Encer -> Amonium Asetat (pH 7.0)
        bool isNeutralizingAmmonia = (sample == CollectableType.AMMONIA_EXTRACT && material == CollectableType.DILUTE_ACETIC_ACID) ||
                                     (sample == CollectableType.DILUTE_ACETIC_ACID && material == CollectableType.AMMONIA_EXTRACT);

        // ==========================================
        // EKSEKUSI HASIL REAKSI
        // ==========================================
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
            resultSlot.SetItem(limedSoilSuspensionIcon, CollectableType.LIMED_SOIL_SUSPENSION);
            KurangiBahan(targetInventory, sampleIndex, materialIndex);
            targetInventory.Add(CollectableType.LIMED_SOIL_SUSPENSION);

            if (QuestManager.Instance != null && QuestManager.Instance.quest1State == MainQuestState.GrindAndExperiment)
            {
                QuestManager.Instance.AdvanceQuest1(MainQuestState.GetJohnApproval);
            }

            ShowReaction(
                "CaCO<sub>3</sub> + 2H<sup>+</sup> → Ca<sup>2+</sup> + CO<sub>2</sub> + H<sub>2</sub>O",
                "Analisis Berhasil! Serbuk kapur (CaCO3) berhasil menetralkan keasaman pada ekstrak tanah. Tunjukkan hasil eksperimen ini kepada John untuk mendapatkan persetujuan sebelum diterapkan ke lahan!"
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
        else if (isDilutingVinegar)
        {
            resultSlot.SetItem(diluteAceticAcidIcon, CollectableType.DILUTE_ACETIC_ACID);
            KurangiBahan(targetInventory, sampleIndex, materialIndex);

            // Menghasilkan 4 botol Asam Asetat Encer (1 untuk uji lab, 3 untuk 3 titik kotoran kandang)
            targetInventory.Add(CollectableType.DILUTE_ACETIC_ACID);
            targetInventory.Add(CollectableType.DILUTE_ACETIC_ACID);
            targetInventory.Add(CollectableType.DILUTE_ACETIC_ACID);
            targetInventory.Add(CollectableType.DILUTE_ACETIC_ACID);

            ShowReaction(
                "CH<sub>3</sub>COOH (pekat) + H<sub>2</sub>O → CH<sub>3</sub>COOH (encer)",
                "Pengenceran Berhasil! Biang cuka pekat telah diencerkan menjadi 3 larutan asam asetat konsentrasi rendah yang aman bagi hewan ternak."
            );
        }
        else if (isExtractingAmmonia)
        {
            resultSlot.SetItem(ammoniaExtractIcon, CollectableType.AMMONIA_EXTRACT);
            KurangiBahan(targetInventory, sampleIndex, materialIndex);
            targetInventory.Add(CollectableType.AMMONIA_EXTRACT);

            ShowReaction(
                "NH<sub>3</sub>(g) + H<sub>2</sub>O(l) ⇌ NH<sub>4</sub><sup>+</sup>(aq) + OH<sup>-</sup>(aq)",
                "Ekstraksi Berhasil! Gas amonia dari sampel kotoran sapi larut dalam air membentuk larutan basa amonia (pH > 9)."
            );
        }
        else if (isNeutralizingAmmonia)
        {
            resultSlot.SetItem(ammoniumAcetateIcon, CollectableType.AMMONIUM_ACETATE);
            KurangiBahan(targetInventory, sampleIndex, materialIndex);
            targetInventory.Add(CollectableType.AMMONIUM_ACETATE);

            if (QuestManager.Instance != null && QuestManager.Instance.quest2State == MainQuest2State.DiluteAndExperiment)
            {
                QuestManager.Instance.AdvanceQuest2(MainQuest2State.GetJohnApproval);
            }

            ShowReaction(
                "NH<sub>3</sub> + CH<sub>3</sub>COOH → CH<sub>3</sub>COONH<sub>4</sub>",
                "Netralisasi Berhasil! Asam asetat encer mengikat amonia basa menjadi garam amonium asetat yang netral (pH 7,0) dan tidak berbau menyengat. Tunjukkan hasil ini kepada John untuk mendapatkan persetujuan!"
            );
        }
        else
        {
            resultSlot.Clear();
            ShowReaction("Reaksi gagal", "Kombinasi bahan kimia tidak cocok.");
        }

        sampleSlot.Clear();
        materialSlot.Clear();

        // Segarkan tampilan Inventory UI sebelah kiri
        if (inventoryUI != null)
            inventoryUI.Setup();
    }

    public void ClearAllSlots()
    {
        if (sampleSlot != null) sampleSlot.Clear();
        if (materialSlot != null) materialSlot.Clear();
        if (resultSlot != null) resultSlot.Clear();
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