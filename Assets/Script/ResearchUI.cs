using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResearchUI : MonoBehaviour
{
    public static ResearchUI Instance { get; private set; }

    [Header("Panel Utama")]
    public GameObject researchPanel;
    public Button closeButton;

    [Header("Riset Awal saat Dibuka")]
    public ResearchData defaultResearch;

    [Header("Tampilan Tengah & Kanan")]
    public Image diagramImage;
    public TextMeshProUGUI processTitleText;
    public TextMeshProUGUI reactionEquationText;
    public TextMeshProUGUI descriptionText;

    [Header("Tombol Aksi Utama (Riset / Proses)")]
    public Button actionButton;
    public TextMeshProUGUI actionButtonText;
    public TextMeshProUGUI actionButtonShadowText;
    public GameObject quarkIconObject;

    [Header("Pop-up Sintesis (Berapa Banyak?)")]
    public GameObject quantityPopup;
    public Button cancelPopupButton;
    public TMP_InputField amountInputField;
    public Button confirmProcessButton;

    [Header("Slot Kebutuhan Bahan di Pop-up (Maksimal 2 Slot)")]
    public GameObject ingredientSlot1;
    public Image ingredientIcon1;
    public TextMeshProUGUI ingredientAmountText1;

    public GameObject ingredientSlot2;
    public Image ingredientIcon2;
    public TextMeshProUGUI ingredientAmountText2;

    [Header("Warna Status Bahan")]
    public Color enoughColor = Color.white;
    public Color notEnoughColor = new Color(1f, 0.35f, 0.35f, 1f);

    private ResearchData currentResearch;
    private HashSet<string> unlockedResearches = new HashSet<string>();
    private int currentCraftAmount = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseUI);

        if (actionButton != null)
            actionButton.onClick.AddListener(OnActionButtonClicked);

        if (cancelPopupButton != null)
            cancelPopupButton.onClick.AddListener(CloseQuantityPopup);

        if (amountInputField != null)
            amountInputField.onValueChanged.AddListener(OnAmountInputChanged);

        if (confirmProcessButton != null)
            confirmProcessButton.onClick.AddListener(ExecuteCraft);

        if (defaultResearch != null)
            SelectResearch(defaultResearch);

        CloseUI();
    }

    public void OpenUI()
    {
        if (researchPanel != null)
        {
            researchPanel.SetActive(true);
            if (currentResearch != null)
                SelectResearch(currentResearch);
        }
    }

    public void CloseUI()
    {
        if (quantityPopup != null)
            quantityPopup.SetActive(false);

        if (researchPanel != null)
            researchPanel.SetActive(false);
    }

    public void SelectResearch(ResearchData data)
    {
        if (data == null) return;
        currentResearch = data;

        processTitleText.text = data.processTitle;
        reactionEquationText.text = data.reactionEquation;
        descriptionText.text = data.description;

        if (data.diagramSprite != null)
            diagramImage.sprite = data.diagramSprite;

        RefreshActionButton();
    }

    private void RefreshActionButton()
    {
        if (currentResearch == null || actionButtonText == null) return;

        bool isResearched = unlockedResearches.Contains(currentResearch.processID);
        string labelText = !isResearched
            ? $"RISET ({currentResearch.researchCost} QUARK)"
            : $"PROSES -> {currentResearch.outputItemName}";

        actionButtonText.text = labelText;

        if (actionButtonShadowText != null)
            actionButtonShadowText.text = labelText;

        if (quarkIconObject != null)
            quarkIconObject.SetActive(!isResearched);
    }

    private void OnActionButtonClicked()
    {
        if (currentResearch == null) return;

        bool isResearched = unlockedResearches.Contains(currentResearch.processID);

        if (!isResearched)
        {
            if (CurrencyManager.Instance != null)
            {
                if (CurrencyManager.Instance.SpendQuark(currentResearch.researchCost))
                {
                    unlockedResearches.Add(currentResearch.processID);
                    RefreshActionButton();
                    Debug.Log($"Berhasil meriset: {currentResearch.processTitle}");
                }
                else
                {
                    Debug.Log("Quark tidak cukup untuk riset!");
                }
            }
            else
            {
                unlockedResearches.Add(currentResearch.processID);
                RefreshActionButton();
            }
        }
        else
        {
            OpenQuantityPopup();
        }
    }

    // =========================================================
    // LOGIKA POP-UP SINTESIS ("BERAPA BANYAK?")
    // =========================================================

    public void OpenQuantityPopup()
    {
        if (quantityPopup == null || currentResearch == null) return;

        currentCraftAmount = 1;
        if (amountInputField != null)
            amountInputField.SetTextWithoutNotify("1");

        quantityPopup.SetActive(true);
        UpdatePopupDisplay();
    }

    public void CloseQuantityPopup()
    {
        if (quantityPopup != null)
            quantityPopup.SetActive(false);
    }

    private void OnAmountInputChanged(string value)
    {
        if (int.TryParse(value, out int parsed))
            currentCraftAmount = Mathf.Max(1, parsed);
        else
            currentCraftAmount = 1;

        UpdatePopupDisplay();
    }

    private void UpdatePopupDisplay()
    {
        if (currentResearch == null) return;

        bool hasEnoughAll = true;
        int slotIndex = 0;

        // 1. Tampilkan Bahan dari Inventory (jika ada)
        if (currentResearch.ingredients != null)
        {
            for (int i = 0; i < currentResearch.ingredients.Count; i++)
            {
                var req = currentResearch.ingredients[i];
                int requiredTotal = req.amountPerCraft * currentCraftAmount;
                int playerStock = (InventoryManager.Instance != null && InventoryManager.Instance.inventory != null)
                    ? InventoryManager.Instance.inventory.GetItemCount(req.itemType)
                    : 0;

                bool isEnough = playerStock >= requiredTotal;
                if (!isEnough) hasEnoughAll = false;

                SetSlotUI(slotIndex, req.icon, $"{playerStock}/{requiredTotal}", isEnough);
                slotIndex++;
            }
        }

        // 2. Tampilkan Biaya Quark di slot ingredient (misal untuk Proses Kontak)
        if (currentResearch.quarkCostPerCraft > 0 && slotIndex < 2)
        {
            int requiredQuark = currentResearch.quarkCostPerCraft * currentCraftAmount;
            int playerQuark = (CurrencyManager.Instance != null) ? CurrencyManager.Instance.CurrentQuark : 0;

            bool isEnough = playerQuark >= requiredQuark;
            if (!isEnough) hasEnoughAll = false;

            SetSlotUI(slotIndex, currentResearch.quarkIcon, $"{playerQuark}/{requiredQuark}", isEnough);
            slotIndex++;
        }

        // 3. Sembunyikan slot sisa yang tidak terpakai
        if (slotIndex <= 0 && ingredientSlot1 != null) ingredientSlot1.SetActive(false);
        if (slotIndex <= 1 && ingredientSlot2 != null) ingredientSlot2.SetActive(false);

        // 4. Aktifkan / nonaktifkan tombol konfirmasi PROSES
        if (confirmProcessButton != null)
            confirmProcessButton.interactable = hasEnoughAll;
    }

    private void SetSlotUI(int index, Sprite icon, string amountText, bool isEnough)
    {
        GameObject slotObj = (index == 0) ? ingredientSlot1 : ingredientSlot2;
        Image iconImg = (index == 0) ? ingredientIcon1 : ingredientIcon2;
        TextMeshProUGUI txt = (index == 0) ? ingredientAmountText1 : ingredientAmountText2;

        if (slotObj != null) slotObj.SetActive(true);
        if (iconImg != null)
        {
            iconImg.sprite = icon;
            iconImg.preserveAspect = true;
        }
        if (txt != null)
        {
            txt.text = amountText;
            txt.color = isEnough ? enoughColor : notEnoughColor;
        }
    }

    private void ExecuteCraft()
    {
        if (currentResearch == null) return;

        // Validasi stok bahan di Inventory
        if (currentResearch.ingredients != null)
        {
            foreach (var req in currentResearch.ingredients)
            {
                int needed = req.amountPerCraft * currentCraftAmount;
                if (InventoryManager.Instance == null ||
                    InventoryManager.Instance.inventory.GetItemCount(req.itemType) < needed)
                {
                    Debug.Log("Bahan di inventory tidak cukup!");
                    return;
                }
            }
        }

        // Validasi & potong Quark (jika proses membutuhkan Quark)
        int totalQuarkNeeded = currentResearch.quarkCostPerCraft * currentCraftAmount;
        if (totalQuarkNeeded > 0)
        {
            if (CurrencyManager.Instance == null || !CurrencyManager.Instance.SpendQuark(totalQuarkNeeded))
            {
                Debug.Log("Quark tidak cukup untuk memproses!");
                return;
            }
        }

        // Potong bahan dari Inventory
        if (currentResearch.ingredients != null && InventoryManager.Instance != null)
        {
            foreach (var req in currentResearch.ingredients)
            {
                int needed = req.amountPerCraft * currentCraftAmount;
                InventoryManager.Instance.inventory.RemoveAmount(req.itemType, needed);
            }
        }

        // Tambahkan produk hasil sintesis ke Inventory
        if (InventoryManager.Instance != null && currentResearch.outputItemType != CollectableType.NONE)
        {
            InventoryManager.Instance.inventory.AddAmount(currentResearch.outputItemType, currentCraftAmount);
            Debug.Log($"Berhasil memproduksi {currentCraftAmount}x {currentResearch.outputItemName}!");
        }

        CloseQuantityPopup();
    }
}