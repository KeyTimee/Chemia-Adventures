using System.Collections.Generic;
using UnityEngine;

public enum MainQuestState
{
    NotStarted,
    GoToFarm,
    CheckSoilPh,
    ReportPhToMax,
    CollectSample,
    ReportSampleToJohn,
    FindLimestone,
    GrindAndExperiment,
    GetJohnApproval,
    NeutralizeSoil,
    ReportFinalToMax,
    Completed
}

public enum MainQuest2State
{
    Locked,                 // Main Quest 1 belum selesai
    WaitingForClara,        // Quest 1 selesai, menunggu 2 hari berlalu
    AvailableAtJohn,        // Sudah >= 2 hari, Clara sudah spawn; pemain cek ke John
    GoToClara,              // John sudah memberi GAS_DETECTOR & mengarahkan ke Clara
    CheckBarnAirPh,         // Clara minta cek 4 titik udara di sekitar kandang sapi
    ReportAirPhToClara,     // 4 titik selesai dicek -> lapor hasil ke Clara
    CollectManureSample,    // Clara minta ambil sampel kotoran sapi (MANURE_SAMPLE)
    ReportSampleToJohn,     // Sampel kotoran diambil -> bawa ke John di Base
    FindVinegarInVillage2,  // John suruh cari Biang Cuka di Village 2 (arah Barat)
    DiluteAndExperiment,    // Cuka dibeli -> encerkan & uji penetralan di Meja Eksperimen
    GetJohnApproval,        // Hasil uji AMMONIUM_ACETATE selesai -> validasi ke John
    NeutralizeBarnAir,      // John menyetujui -> semprot DILUTE_ACETIC_ACID di 2 titik kandang
    ReportFinalToClara,     // 2 titik disemprot -> lapor & edukasi Clara
    Completed               // Main Quest 2 selesai!
}

public enum DailyQuestState
{
    NotAvailable,   // Belum terbuka (Main Quest 1 belum selesai)
    Available,      // Siap diambil dari Max
    WateringTask,   // Pemain sedang menyiram 4 titik di 2 petak lahan kanan
    ReadyToReport,  // 4 titik selesai disiram -> kembali ke Max
    CompletedToday  // Selesai untuk hari ini (menunggu ganti hari)
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Main Quest 1")]
    public MainQuestState quest1State = MainQuestState.NotStarted;
    public int totalBlocksToCheck = 4;
    public int totalAcidicBlocksToHeal = 2;

    private HashSet<string> checkedBlockIDs = new HashSet<string>();
    private HashSet<string> neutralizedBlockIDs = new HashSet<string>();

    [Header("Main Quest 2 (Amonia Kandang Sapi)")]
    public MainQuest2State quest2State = MainQuest2State.Locked;
    public int quest1CompletedDay = -1;
    public int daysRequiredForClara = 2;
    public int totalAirPointsToCheck = 5;
    public int totalAmmoniaPointsToHeal = 3;

    private HashSet<string> checkedAirPointIDs = new HashSet<string>();
    private HashSet<string> neutralizedAirPointIDs = new HashSet<string>();

    [Header("Daily Quest (Siram 4 Titik Lahan Normal)")]
    public DailyQuestState dailyQuestState = DailyQuestState.NotAvailable;
    public int wateredSoilCount = 0;
    public int targetWaterCount = 4; // 2 titik di kanan atas + 2 titik di kanan bawah

    private HashSet<string> wateredPointIDs = new HashSet<string>();

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

    // ==========================================
    // LOGIKA MAIN QUEST 1
    // ==========================================

    public void AdvanceQuest1(MainQuestState newState)
    {
        quest1State = newState;
        Debug.Log($"[Main Quest 1] State: {quest1State}");

        if (quest1State == MainQuestState.Completed)
        {
            // 1. Buka Daily Quest di Max
            if (dailyQuestState == DailyQuestState.NotAvailable)
            {
                dailyQuestState = DailyQuestState.Available;
                Debug.Log("[Daily Quest] Sekarang TERBUKA di Max!");
            }

            // 2. Catat hari selesai Quest 1 untuk timer spawn Clara (2 hari kemudian)
            if (quest1CompletedDay < 0)
            {
                quest1CompletedDay = DayManager.Instance != null ? DayManager.Instance.currentDay : 1;
                quest2State = MainQuest2State.WaitingForClara;
                Debug.Log($"[Main Quest 2] Quest 1 selesai di Day {quest1CompletedDay}. Clara akan spawn pada Day {quest1CompletedDay + daysRequiredForClara}.");
            }
        }
    }

    public void RegisterCheckedBlock(string blockID)
    {
        if (checkedBlockIDs.Contains(blockID)) return;

        checkedBlockIDs.Add(blockID);
        if (checkedBlockIDs.Count >= totalBlocksToCheck && quest1State == MainQuestState.CheckSoilPh)
        {
            AdvanceQuest1(MainQuestState.ReportPhToMax);
        }
    }

    public bool IsBlockChecked(string blockID) => checkedBlockIDs.Contains(blockID);

    public void RegisterNeutralizedBlock(string blockID)
    {
        if (neutralizedBlockIDs.Contains(blockID)) return;

        neutralizedBlockIDs.Add(blockID);
        if (neutralizedBlockIDs.Count >= totalAcidicBlocksToHeal && quest1State == MainQuestState.NeutralizeSoil)
        {
            AdvanceQuest1(MainQuestState.ReportFinalToMax);
        }
    }

    public bool IsBlockNeutralized(string blockID) => neutralizedBlockIDs.Contains(blockID);

    // ==========================================
    // LOGIKA MAIN QUEST 2 & SPAWN CLARA
    // ==========================================

    public void AdvanceQuest2(MainQuest2State newState)
    {
        quest2State = newState;
        Debug.Log($"[Main Quest 2] State: {quest2State}");
    }

    // Dipanggil oleh DayManager setiap hari bertambah, dan juga saat cek ke John / load scene Farm 1
    public void CheckQuest2UnlockCondition()
    {
        if (quest1State != MainQuestState.Completed) return;
        if (quest2State != MainQuest2State.WaitingForClara) return;

        int currentDay = DayManager.Instance != null ? DayManager.Instance.currentDay : 1;
        if (quest1CompletedDay > 0 && (currentDay - quest1CompletedDay) >= daysRequiredForClara)
        {
            AdvanceQuest2(MainQuest2State.AvailableAtJohn);
            Debug.Log("[Main Quest 2] 2 hari telah berlalu! Clara sudah spawn di Farm 1. Pemain bisa cek ke John.");
        }
    }

    // Menentukan apakah GameObject Clara di scene Farm 1 harus muncul (active)
    public bool ShouldClaraBeVisible()
    {
        CheckQuest2UnlockCondition();
        return quest1State == MainQuestState.Completed &&
               quest2State >= MainQuest2State.AvailableAtJohn;
    }

    public int GetRemainingDaysForClara()
    {
        if (quest1CompletedDay < 0) return daysRequiredForClara;
        int currentDay = DayManager.Instance != null ? DayManager.Instance.currentDay : 1;
        return Mathf.Max(0, daysRequiredForClara - (currentDay - quest1CompletedDay));
    }

    public void RegisterCheckedAirPoint(string pointID)
    {
        if (checkedAirPointIDs.Contains(pointID)) return;

        checkedAirPointIDs.Add(pointID);
        Debug.Log($"[Main Quest 2] Titik udara dicek: {checkedAirPointIDs.Count}/{totalAirPointsToCheck}");

        if (checkedAirPointIDs.Count >= totalAirPointsToCheck && quest2State == MainQuest2State.CheckBarnAirPh)
        {
            AdvanceQuest2(MainQuest2State.ReportAirPhToClara);
        }
    }

    public bool IsAirPointChecked(string pointID) => checkedAirPointIDs.Contains(pointID);
    public int GetCheckedAirPointCount() => checkedAirPointIDs.Count;

    public void RegisterNeutralizedAirPoint(string pointID)
    {
        if (neutralizedAirPointIDs.Contains(pointID)) return;

        neutralizedAirPointIDs.Add(pointID);
        Debug.Log($"[Main Quest 2] Titik kandang dinetralkan: {neutralizedAirPointIDs.Count}/{totalAmmoniaPointsToHeal}");

        if (neutralizedAirPointIDs.Count >= totalAmmoniaPointsToHeal && quest2State == MainQuest2State.NeutralizeBarnAir)
        {
            AdvanceQuest2(MainQuest2State.ReportFinalToClara);
        }
    }

    public bool IsAirPointNeutralized(string pointID) => neutralizedAirPointIDs.Contains(pointID);
    public int GetNeutralizedAirPointCount() => neutralizedAirPointIDs.Count;

    // ==========================================
    // LOGIKA DAILY QUEST (MENYIRAM 4 TITIK KANAN)
    // ==========================================

    public void StartDailyWateringQuest()
    {
        dailyQuestState = DailyQuestState.WateringTask;
        wateredSoilCount = 0;
        wateredPointIDs.Clear();
    }

    public bool RegisterWateredPoint(string pointID)
    {
        if (dailyQuestState != DailyQuestState.WateringTask) return false;
        if (wateredPointIDs.Contains(pointID)) return false;

        wateredPointIDs.Add(pointID);
        wateredSoilCount = wateredPointIDs.Count;
        Debug.Log($"[Daily Quest] Disiram: {wateredSoilCount}/{targetWaterCount}");

        if (wateredSoilCount >= targetWaterCount)
        {
            dailyQuestState = DailyQuestState.ReadyToReport;
            Debug.Log("[Daily Quest] Semua 4 titik selesai disiram! Kembali ke Max.");
        }
        return true;
    }

    public bool IsPointWatered(string pointID) => wateredPointIDs.Contains(pointID);

    // Panggil fungsi ini dari Time/Day Manager kamu setiap hari bertambah!
    public void ResetDailyQuest()
    {
        if (quest1State != MainQuestState.Completed) return;

        dailyQuestState = DailyQuestState.Available;
        wateredSoilCount = 0;
        wateredPointIDs.Clear();
        Debug.Log("[Daily Quest] Hari baru dimulai! Quest harian telah di-reset dan tersedia kembali di Max.");
    }
}