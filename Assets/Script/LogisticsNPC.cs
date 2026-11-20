using UnityEngine;

public class LogisticsNPC : MonoBehaviour
{
    [Header("Pengaturan Interaksi")]
    public KeyCode interactKey = KeyCode.E;
    public GameObject interactPromptIcon; // Opsional: ikon "[E]" di atas kepala NPC

    private bool isPlayerInRange = false;

    private void Start()
    {
        if (interactPromptIcon != null)
            interactPromptIcon.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(interactKey))
        {
            // Buka UI Research & Development jika belum terbuka
            if (ResearchUI.Instance != null)
            {
                ResearchUI.Instance.OpenUI();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactPromptIcon != null)
                interactPromptIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactPromptIcon != null)
                interactPromptIcon.SetActive(false);
        }
    }
}