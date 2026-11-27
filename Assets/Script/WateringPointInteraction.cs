using UnityEngine;

public class WaterPointInteraction : MonoBehaviour
{
    [Header("Identitas Titik Siram")]
    public string pointID = "Water_KananAtas_1"; // Wajib unik di tiap 4 titik!

    [Header("UI & Visual")]
    public Interact_UI interactUI;
    public GameObject locationIcon;       // Ikon target menyala saat belum disiram
    public GameObject wetSoilVisual;      // Opsional: visual tanah basah setelah disiram

    [Header("Dialog")]
    public DialogueData needWateringCanDialogue; // Jika pemain belum bawa Watering Can
    public DialogueData allWateredDialogue;      // Muncul otomatis saat titik ke-4 selesai disiram!

    private bool isPlayerInRange = false;
    private Player player;

    private void Start()
    {
        if (interactUI != null) interactUI.Hide();
    }

    private void Update()
    {
        bool isActiveTarget = IsTargetActiveNow();

        if (locationIcon != null)
            locationIcon.SetActive(isActiveTarget);

        if (wetSoilVisual != null && QuestManager.Instance != null)
            wetSoilVisual.SetActive(QuestManager.Instance.IsPointWatered(pointID));

        if (!isActiveTarget && interactUI != null)
            interactUI.Hide();

        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            return;

        if (isPlayerInRange && isActiveTarget && Input.GetKeyDown(KeyCode.E))
        {
            WaterThisSpot();
        }
    }

    private bool IsTargetActiveNow()
    {
        if (QuestManager.Instance == null) return false;

        return QuestManager.Instance.dailyQuestState == DailyQuestState.WateringTask &&
               !QuestManager.Instance.IsPointWatered(pointID);
    }

    private void WaterThisSpot()
    {
        if (QuestManager.Instance == null || player == null) return;

        // Pastikan pemain membawa Watering Can
        if (!player.inventory.Contains(CollectableType.WATERING_CAN))
        {
            if (DialogueManager.Instance != null && needWateringCanDialogue != null)
                DialogueManager.Instance.StartDialogue(needWateringCanDialogue);
            return;
        }

        // Catat penyiraman
        bool success = QuestManager.Instance.RegisterWateredPoint(pointID);

        // Jika setelah menyiram titik ini ke-4 titik selesai -> munculkan dialog kembali ke Max!
        if (success && QuestManager.Instance.dailyQuestState == DailyQuestState.ReadyToReport)
        {
            if (DialogueManager.Instance != null && allWateredDialogue != null)
                DialogueManager.Instance.StartDialogue(allWateredDialogue);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player p = collision.GetComponent<Player>();
        if (p != null)
        {
            player = p;
            isPlayerInRange = true;

            if (IsTargetActiveNow() && interactUI != null)
                interactUI.Show("Press E");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player p = collision.GetComponent<Player>();
        if (p != null)
        {
            isPlayerInRange = false;
            player = null;
            if (interactUI != null) interactUI.Hide();
        }
    }
}