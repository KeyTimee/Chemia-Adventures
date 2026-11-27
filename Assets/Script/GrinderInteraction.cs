using UnityEngine;

public class GrinderTable : MonoBehaviour
{
    [Header("UI References (Tarik dari Inspector)")]
    public GameObject grinderUI;
    public Inventory_UI grinderInventoryUI;
    public GrinderSystem grinderSystem;
    public GameObject interactText;

    private bool playerInRange = false;

    void Start()
    {
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

        if (grinderInventoryUI == null && grinderUI != null)
            grinderInventoryUI = grinderUI.GetComponentInChildren<Inventory_UI>(true);

        if (grinderSystem == null && grinderUI != null)
            grinderSystem = grinderUI.GetComponentInChildren<GrinderSystem>(true);

        if (interactText == null)
        {
            GameObject foundText = GameObject.FindWithTag("InteractText");
            if (foundText == null) foundText = GameObject.Find("InteractText");
            if (foundText != null) interactText = foundText;
        }

        if (grinderUI != null) grinderUI.SetActive(false);
        if (interactText != null) interactText.SetActive(false);
    }

    void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            return;

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (grinderUI != null)
            {
                if (!grinderUI.activeSelf)
                    BukaUI();
            }
            else
            {
                Debug.LogError("Gagal Membuka: Variable 'grinderUI' masih NULL! Tarik GameObject GrinderUI ke Inspector.");
            }
        }

        if (grinderUI != null && grinderUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            TutupUI();
        }
    }

    public void BukaUI()
    {
        if (grinderSystem != null)
            grinderSystem.ClearAllSlots();

        if (grinderUI != null)
            grinderUI.SetActive(true);

        if (grinderInventoryUI != null)
        {
            if (grinderInventoryUI.inventoryPanel != null)
                grinderInventoryUI.inventoryPanel.SetActive(true);

            grinderInventoryUI.Setup();
        }

        if (interactText != null)
            interactText.SetActive(false);
    }

    public void TutupUI()
    {
        if (grinderSystem != null)
            grinderSystem.ClearAllSlots();

        if (grinderUI != null)
            grinderUI.SetActive(false);

        if (grinderInventoryUI != null && grinderInventoryUI.inventoryPanel != null)
            grinderInventoryUI.inventoryPanel.SetActive(false);

        if (playerInRange && interactText != null)
            interactText.SetActive(true);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactText != null && (grinderUI == null || !grinderUI.activeSelf))
                interactText.SetActive(true);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactText != null)
                interactText.SetActive(false);

            if (grinderUI != null && grinderUI.activeSelf)
                TutupUI();
        }
    }
}