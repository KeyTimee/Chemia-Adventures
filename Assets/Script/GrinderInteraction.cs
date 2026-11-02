using UnityEngine;

public class GrinderTable : MonoBehaviour
{
    [Header("UI References (Tarik dari Inspector)")]
    public GameObject grinderUI;
    public Inventory_UI grinderInventoryUI;
    public GameObject interactText;

    private bool playerInRange = false;

    void Start()
    {
        // 1. Cari GrinderUI jika belum di-drag di Inspector
        if (grinderUI == null)
        {
            Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
            foreach (Transform t in allTransforms)
            {
                if (t.name == "GrinderUI" && t.gameObject.scene.name == gameObject.scene.name)
                {
                    grinderUI = t.gameObject;
                    break;
                }
            }
        }

        // 2. Cari Inventory_UI di Left_Inventory milik UI Grinder
        if (grinderInventoryUI == null && grinderUI != null)
        {
            grinderInventoryUI = grinderUI.GetComponentInChildren<Inventory_UI>(true);
        }

        // 3. Cari InteractText di Scene
        if (interactText == null)
        {
            GameObject foundText = GameObject.FindWithTag("InteractText");
            if (foundText == null) foundText = GameObject.Find("InteractText");
            if (foundText != null) interactText = foundText;
        }

        // Sembunyikan UI saat awal
        if (grinderUI != null) grinderUI.SetActive(false);
        if (interactText != null) interactText.SetActive(false);
    }

    void Update()
    {
        // Buka UI dengan Tombol 'E'
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (grinderUI != null)
            {
                if (!grinderUI.activeSelf)
                {
                    BukaUI();
                }
            }
            else
            {
                Debug.LogError("Gagal Membuka: Variable 'grinderUI' masih NULL! Tarik GameObject GrinderUI ke Inspector.");
            }
        }

        // Tutup UI dengan Tombol 'Escape'
        if (grinderUI != null && grinderUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            TutupUI();
        }
    }

    public void BukaUI()
    {
        Debug.Log("Membuka UI Grinder...");

        if (grinderUI != null)
        {
            grinderUI.SetActive(true);
        }

        if (grinderInventoryUI != null)
        {
            if (grinderInventoryUI.inventoryPanel != null)
            {
                grinderInventoryUI.inventoryPanel.SetActive(true);
            }

            // Memperbarui slot inventory sebelah kiri
            grinderInventoryUI.Setup();
        }

        if (interactText != null)
        {
            interactText.SetActive(false);
        }
    }

    public void TutupUI()
    {
        Debug.Log("Menutup UI Grinder...");

        if (grinderUI != null)
        {
            grinderUI.SetActive(false);
        }

        if (grinderInventoryUI != null && grinderInventoryUI.inventoryPanel != null)
        {
            grinderInventoryUI.inventoryPanel.SetActive(false);
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
            Debug.Log("Pemain BERHASIL masuk area interaksi Meja Grinder!");

            if (interactText != null && (grinderUI == null || !grinderUI.activeSelf))
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

            if (grinderUI != null && grinderUI.activeSelf)
            {
                TutupUI();
            }
        }
    }
}