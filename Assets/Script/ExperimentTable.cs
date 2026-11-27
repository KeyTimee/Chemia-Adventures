using UnityEngine;

public class ExperimentTable : MonoBehaviour
{
    [Header("UI References (Tarik dari Inspector)")]
    public GameObject experimentUI;
    public Inventory_UI experimentInventoryUI;
    public ExperimentSystem experimentSystem;
    public GameObject interactText;

    private bool playerInRange = false;

    void Start()
    {
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

        if (experimentInventoryUI == null && experimentUI != null)
            experimentInventoryUI = experimentUI.GetComponentInChildren<Inventory_UI>(true);

        if (experimentSystem == null && experimentUI != null)
            experimentSystem = experimentUI.GetComponentInChildren<ExperimentSystem>(true);

        if (interactText == null)
        {
            GameObject foundText = GameObject.FindWithTag("InteractText");
            if (foundText == null) foundText = GameObject.Find("InteractText");
            if (foundText != null) interactText = foundText;
        }

        if (experimentUI != null) experimentUI.SetActive(false);
        if (interactText != null) interactText.SetActive(false);
    }

    void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            return;

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (experimentUI != null)
            {
                if (!experimentUI.activeSelf)
                    BukaUI();
            }
            else
            {
                Debug.LogError("Gagal Membuka: Variable 'experimentUI' masih NULL! Tarik GameObject Experiment ke Inspector.");
            }
        }

        if (experimentUI != null && experimentUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            TutupUI();
        }
    }

    public void BukaUI()
    {
        if (experimentSystem != null)
            experimentSystem.ClearAllSlots();

        if (experimentUI != null)
            experimentUI.SetActive(true);

        if (experimentInventoryUI != null)
        {
            if (experimentInventoryUI.inventoryPanel != null)
                experimentInventoryUI.inventoryPanel.SetActive(true);

            experimentInventoryUI.Setup();
        }

        if (interactText != null)
            interactText.SetActive(false);
    }

    public void TutupUI()
    {
        if (experimentSystem != null)
            experimentSystem.ClearAllSlots();

        if (experimentUI != null)
            experimentUI.SetActive(false);

        if (experimentInventoryUI != null && experimentInventoryUI.inventoryPanel != null)
            experimentInventoryUI.inventoryPanel.SetActive(false);

        if (playerInRange && interactText != null)
            interactText.SetActive(true);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactText != null && (experimentUI == null || !experimentUI.activeSelf))
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

            if (experimentUI != null && experimentUI.activeSelf)
                TutupUI();
        }
    }
}