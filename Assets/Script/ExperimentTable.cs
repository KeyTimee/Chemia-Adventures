using UnityEngine;

public class ExperimentTable : MonoBehaviour
{
    [Header("UI References (Tarik dari Inspector)")]
    public GameObject experimentUI;
    public Inventory_UI experimentInventoryUI;
    public GameObject interactText;

    private bool playerInRange = false;

    void Start()
    {
        // 1. Cari ExperimentUI jika belum di-drag di Inspector
        if (experimentUI == null)
        {
            Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
            foreach (Transform t in allTransforms)
            {
                if (t.name == "Experiment" && t.gameObject.scene.name == gameObject.scene.name)
                {
                    experimentUI = t.gameObject;
                    break;
                }
            }
        }

        // 2. Cari Inventory_UI di Left_Inventory
        if (experimentInventoryUI == null && experimentUI != null)
        {
            experimentInventoryUI = experimentUI.GetComponentInChildren<Inventory_UI>(true);
        }

        // 3. Cari InteractText di Scene
        if (interactText == null)
        {
            GameObject foundText = GameObject.FindWithTag("InteractText");
            if (foundText == null) foundText = GameObject.Find("InteractText");
            if (foundText != null) interactText = foundText;
        }

        // Sembunyikan UI saat awal
        if (experimentUI != null) experimentUI.SetActive(false);
        if (interactText != null) interactText.SetActive(false);
    }

    void Update()
    {
        // Pilihan Buka UI dengan Tombol 'E'
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (experimentUI != null)
            {
                // Jika UI sedang tidak aktif, Buka UI
                if (!experimentUI.activeSelf)
                {
                    BukaUI();
                }
            }
            else
            {
                Debug.LogError("Gagal Membuka: Variable 'experimentUI' masih NULL! Tarik GameObject Experiment ke Inspector.");
            }
        }

        // Tutup UI dengan Tombol 'Escape'
        if (experimentUI != null && experimentUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            TutupUI();
        }
    }

    public void BukaUI()
    {
        Debug.Log("Membuka UI Experiment...");

        if (experimentUI != null)
        {
            experimentUI.SetActive(true);
        }

        if (experimentInventoryUI != null)
        {
            if (experimentInventoryUI.inventoryPanel != null)
            {
                experimentInventoryUI.inventoryPanel.SetActive(true);
            }

            // Memperbarui slot inventory sebelah kiri
            experimentInventoryUI.Setup();
        }

        if (interactText != null)
        {
            interactText.SetActive(false);
        }
    }

    public void TutupUI()
    {
        Debug.Log("Menutup UI Experiment...");

        if (experimentUI != null)
        {
            experimentUI.SetActive(false);
        }

        if (experimentInventoryUI != null && experimentInventoryUI.inventoryPanel != null)
        {
            experimentInventoryUI.inventoryPanel.SetActive(false);
        }

        if (playerInRange && interactText != null)
        {
            interactText.SetActive(true);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Pemain BERHASIL masuk area interaksi meja!");

            if (interactText != null && (experimentUI == null || !experimentUI.activeSelf))
            {
                interactText.SetActive(true);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactText != null)
            {
                interactText.SetActive(false);
            }

            if (experimentUI != null && experimentUI.activeSelf)
            {
                TutupUI();
            }
        }
    }
}