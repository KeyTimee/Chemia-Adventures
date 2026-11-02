using UnityEngine;

// 🔥 Enum untuk Quest Utama 1
public enum MainQuestState
{
    NotStarted,
    QuestStarted,
    SampleCollected,
    ExperimentDone,
    Completed
}

// Enum untuk Daily Quest
public enum DailyQuestState
{
    NotAvailable,   // Belum terbuka (Main Quest 1 belum selesai)
    Available,      // Quest siap diambil dari NPC
    WateringTask,   // Pemain sedang bertugas menyiram 8 titik
    ReadyToReport,  // 8 titik selesai disiram, saatnya kembali ke NPC
    CompletedToday  // Quest harian sudah selesai untuk hari ini
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Main Quest 1")]
    public MainQuestState quest1State = MainQuestState.NotStarted;

    [Header("Daily Quest")]
    public DailyQuestState dailyQuestState = DailyQuestState.NotAvailable;
    public int wateredSoilCount = 0;
    public int targetWaterCount = 8; // Target 8 titik lokasi

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AdvanceQuest1(MainQuestState newState)
    {
        quest1State = newState;
        Debug.Log($"Main Quest 1 State Updated: {quest1State}");

        // Jika Quest Utama 1 Selesai, otomatis buka akses Daily Quest!
        if (quest1State == MainQuestState.Completed && dailyQuestState == DailyQuestState.NotAvailable)
        {
            dailyQuestState = DailyQuestState.Available;
            Debug.Log("Daily Side Quest Sekarang TERBUKA!");
        }
    }

    // Fungsi menambah progres penyiraman
    public void AddWateredCount()
    {
        wateredSoilCount++;
        Debug.Log($"[Daily Quest] Disiram: {wateredSoilCount} / {targetWaterCount}");

        if (wateredSoilCount >= targetWaterCount)
        {
            dailyQuestState = DailyQuestState.ReadyToReport;
            Debug.Log("[Daily Quest] Semua 8 titik selesai disiram! Kembali ke NPC Farmer.");
        }
    }
}