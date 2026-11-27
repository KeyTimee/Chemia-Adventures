/*using UnityEngine;

public class WateringCanPickup : MonoBehaviour
{
    [Header("References")]
    public GameObject interactText;

    private bool isPlayerInRange = false;
    private Player player;

    private void Start()
    {
        if (interactText != null) interactText.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TakeWateringCan();
        }
    }

    private void TakeWateringCan()
    {
        if (player == null || QuestManager.Instance == null) return;

        // Hanya bisa diambil jika pemain sedang berada di tahap WateringTask
        if (QuestManager.Instance.dailyQuestState == DailyQuestState.WateringTask)
        {
            if (!player.inventory.Contains(CollectableType.WATERING_CAN))
            {
                player.inventory.Add(CollectableType.WATERING_CAN);
                Debug.Log("Mengambil Watering Can dari rak!");
                if (interactText != null) interactText.SetActive(false);
            }
            else
            {
                Debug.Log("Kamu sudah membawa Watering Can di inventory!");
            }
        }
        else
        {
            Debug.Log("Ambil quest dari NPC Farmer terlebih dahulu!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player p = collision.GetComponent<Player>();
        if (p != null)
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
}*/