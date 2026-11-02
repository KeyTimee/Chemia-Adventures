using UnityEngine;

public class FarmNPC : MonoBehaviour
{
    [Header("UI Reference")]
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
            InteractWithNPC();
        }
    }

    private void InteractWithNPC()
    {
        if (QuestManager.Instance == null || player == null) return;

        // 1. CEK KEMAJUAN MAIN QUEST 1 DULU
        if (QuestManager.Instance.quest1State != MainQuestState.Completed)
        {
            HandleMainQuestDialog();
            return;
        }

        // 2. JIKA MAIN QUEST 1 SUDAH SELESAI -> MASUK LOGIKA DAILY QUEST
        HandleDailyQuestDialog();
    }

    private void HandleMainQuestDialog()
    {
        MainQuestState state = QuestManager.Instance.quest1State;
        switch (state)
        {
            case MainQuestState.NotStarted:
                QuestManager.Instance.AdvanceQuest1(MainQuestState.QuestStarted);
                Debug.Log("NPC: 'Tolong bantu saya! Ambilkan sampel tanah di titik lokasi!'");
                break;
            case MainQuestState.QuestStarted:
                Debug.Log("NPC: 'Ambil sampel tanah di area perkebunan dulu ya!'");
                break;
            case MainQuestState.SampleCollected:
                Debug.Log("NPC: 'Bawa sampel tanah itu ke Meja Eksperimen untuk diuji!'");
                break;
            case MainQuestState.ExperimentDone:
                Debug.Log("NPC: 'Taburkan CaCO3 yang sudah diuji ke tanah perkebunan!'");
                break;
        }
    }

    private void HandleDailyQuestDialog()
    {
        DailyQuestState dailyState = QuestManager.Instance.dailyQuestState;

        switch (dailyState)
        {
            case DailyQuestState.Available:
                // Terima Daily Quest
                QuestManager.Instance.dailyQuestState = DailyQuestState.WateringTask;
                QuestManager.Instance.wateredSoilCount = 0; // Reset counter
                Debug.Log("NPC: 'Halo! Bisakah kamu membantuku menyiram 8 titik lahan perkebunan hari ini? Ambil Watering Can di sampingku!'");
                break;

            case DailyQuestState.WateringTask:
                Debug.Log($"NPC: 'Siram 8 titik tanaman ya! Baru disiram {QuestManager.Instance.wateredSoilCount}/8.'");
                break;

            case DailyQuestState.ReadyToReport:
                // Selesaikan Daily Quest & Kembalikan Watering Can
                if (player.inventory.Contains(CollectableType.WATERING_CAN))
                {
                    player.inventory.Remove(CollectableType.WATERING_CAN);
                }

                // Beri Imbalan 250 Quark!
                if (CurrencyManager.Instance != null)
                {
                    CurrencyManager.Instance.AddQuark(250);
                    Debug.Log("Uang 250 Quark berhasil ditambahkan!");
                }
                else
                {
                    Debug.LogError("CurrencyManager.Instance masih NULL! Pastikan GameObject CurrencyManager sudah ada di scene.");
                }

                QuestManager.Instance.dailyQuestState = DailyQuestState.CompletedToday;
                Debug.Log("NPC: 'Terima kasih banyak! Ini imbalan 250 Quark untuk kerja kerasmu hari ini.'");
                break;

            case DailyQuestState.CompletedToday:
                Debug.Log("NPC: 'Pekerjaan hari ini sudah selesai! Kembalilah besok untuk pekerjaan harian lainnya.'");
                break;
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
}