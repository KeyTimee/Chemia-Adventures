using UnityEngine;

public class ExperimentTable : MonoBehaviour
{
    public GameObject experimentUI;
    public Inventory_UI inventoryUI;
    public GameObject interactText;

    private bool playerInRange = false;

    void Update()
    {
        // 🔥 BUKA UI (tekan E)
        if (playerInRange && !experimentUI.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            experimentUI.SetActive(true);
            inventoryUI.inventoryPanel.SetActive(true);
            inventoryUI.Setup();

            interactText.SetActive(false);
        }

        // 🔥 TUTUP UI (tekan ESC)
        if (experimentUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            experimentUI.SetActive(false);
            inventoryUI.inventoryPanel.SetActive(false);

            // kalau masih di dekat meja → munculin lagi text
            if (playerInRange)
            {
                interactText.SetActive(true);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (!experimentUI.activeSelf)
            {
                interactText.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactText.SetActive(false);
        }
    }

    void Start()
    {
        experimentUI.SetActive(false);
        interactText.SetActive(false);
    }
}