using UnityEngine;

public class WateringPoint : MonoBehaviour
{
    [Header("UI & Visual Reference")]
    public GameObject interactText;
    public GameObject locationIcon; // Icon animasi bracket target
    public SpriteRenderer soilSpriteRenderer;
    public Sprite wateredSoilSprite; // (Opsional) Sprite tanah basah setelah disiram

    private bool isWatered = false;
    private bool isPlayerInRange = false;
    private Player player;

    private void Start()
    {
        if (interactText != null) interactText.SetActive(false);
    }

    private void Update()
    {
        UpdateVisibility();

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            WaterSoil();
        }
    }

    private void UpdateVisibility()
    {
        if (QuestManager.Instance == null) return;

        // Icon target hanya muncul jika Daily Quest aktif dan tanah belum disiram
        bool isQuestActive = (QuestManager.Instance.dailyQuestState == DailyQuestState.WateringTask);
        
        if (locationIcon != null)
        {
            locationIcon.SetActive(isQuestActive && !isWatered);
        }
    }

    private void WaterSoil()
    {
        if (isWatered || player == null || QuestManager.Instance == null) return;

        // Cek apakah quest harian sedang aktif
        if (QuestManager.Instance.dailyQuestState != DailyQuestState.WateringTask)
        {
            Debug.Log("Tidak ada alasan menyiram tanah ini sekarang.");
            return;
        }

        // Cek apakah pemain membawa WATERING_CAN
        if (player.inventory.Contains(CollectableType.WATERING_CAN))
        {
            isWatered = true;

            // Ganti visual tanah jika ada sprite tanah basah
            if (soilSpriteRenderer != null && wateredSoilSprite != null)
            {
                soilSpriteRenderer.sprite = wateredSoilSprite;
            }

            // Tambah progres di QuestManager
            QuestManager.Instance.AddWateredCount();

            if (interactText != null) interactText.SetActive(false);
            if (locationIcon != null) locationIcon.SetActive(false);

            Debug.Log("Tanah berhasil disiram!");
        }
        else
        {
            Debug.Log("Kamu perlu mengambil Watering Can di dekat NPC Farmer dulu!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player p = collision.GetComponent<Player>();
        if (p != null && !isWatered)
        {
            player = p;
            isPlayerInRange = true;
            if (interactText != null) interactText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            isPlayerInRange = false;
            player = null;
            if (interactText != null) interactText.SetActive(false);
        }
    }
}