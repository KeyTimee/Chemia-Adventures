using UnityEngine;

public class FarmNPC : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject interactText;

    [Header("Dialog Main Quest 1")]
    public DialogueData max_BeforeBriefing;
    public DialogueData max_IntroProblem;
    public DialogueData max_ReminderCheckPh;
    public DialogueData max_AmmoniumExplain;
    public DialogueData max_ReminderSample;
    public DialogueData max_ReminderNeutralize;
    public DialogueData max_FinalEducation;

    [Header("Dialog Daily Quest (4 Titik Lahan Kanan)")]
    public DialogueData daily_OfferDialogue;       // Max menawarkan kerja harian (upah 250 Quark)
    public DialogueData daily_InProgressDialogue;  // Pengingat siram 4 titik di petak kanan
    public DialogueData daily_RewardDialogue;      // Terima kasih & beri 250 Quark
    public DialogueData daily_CompletedDialogue;   // Sudah selesai hari ini, kembali besok

    [Header("Hadiah Quest")]
    public int mainQuestRewardQuark = 300;
    public int dailyQuestRewardQuark = 250;
    public bool giveWateringCanOnAccept = true; // Centang jika Watering Can otomatis diberikan saat terima quest

    private bool isPlayerInRange = false;
    private Player player;

    private void Start()
    {
        if (interactText != null) interactText.SetActive(false);
    }

    private void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            return;

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            InteractWithMax();
        }
    }

    private void InteractWithMax()
    {
        if (QuestManager.Instance == null || player == null) return;

        // 1. Jika Main Quest 1 belum selesai, jalankan alur Main Quest 1
        if (QuestManager.Instance.quest1State != MainQuestState.Completed)
        {
            HandleMainQuest();
            return;
        }

        // 2. Jika Main Quest 1 sudah selesai, masuk ke Daily Quest
        HandleDailyQuest();
    }

    private void HandleMainQuest()
    {
        MainQuestState state = QuestManager.Instance.quest1State;

        switch (state)
        {
            case MainQuestState.NotStarted:
                PlayDialogue(max_BeforeBriefing);
                break;

            case MainQuestState.GoToFarm:
                QuestManager.Instance.AdvanceQuest1(MainQuestState.CheckSoilPh);
                PlayDialogue(max_IntroProblem);
                break;

            case MainQuestState.CheckSoilPh:
                PlayDialogue(max_ReminderCheckPh);
                break;

            case MainQuestState.ReportPhToMax:
                QuestManager.Instance.AdvanceQuest1(MainQuestState.CollectSample);
                PlayDialogue(max_AmmoniumExplain);
                break;

            case MainQuestState.CollectSample:
            case MainQuestState.ReportSampleToJohn:
            case MainQuestState.FindLimestone:
            case MainQuestState.GrindAndExperiment:
            case MainQuestState.GetJohnApproval:
                PlayDialogue(max_ReminderSample);
                break;

            case MainQuestState.NeutralizeSoil:
                PlayDialogue(max_ReminderNeutralize);
                break;

            case MainQuestState.ReportFinalToMax:
                QuestManager.Instance.AdvanceQuest1(MainQuestState.Completed);
                if (CurrencyManager.Instance != null)
                    CurrencyManager.Instance.AddQuark(mainQuestRewardQuark);

                PlayDialogue(max_FinalEducation);
                break;
        }
    }

    private void HandleDailyQuest()
    {
        DailyQuestState dailyState = QuestManager.Instance.dailyQuestState;

        switch (dailyState)
        {
            case DailyQuestState.Available:
                QuestManager.Instance.StartDailyWateringQuest();

                if (giveWateringCanOnAccept && !player.inventory.Contains(CollectableType.WATERING_CAN))
                {
                    player.inventory.Add(CollectableType.WATERING_CAN);
                }

                PlayDialogue(daily_OfferDialogue);
                break;

            case DailyQuestState.WateringTask:
                PlayDialogue(daily_InProgressDialogue);
                break;

            case DailyQuestState.ReadyToReport:
                if (player.inventory.Contains(CollectableType.WATERING_CAN))
                {
                    player.inventory.Remove(CollectableType.WATERING_CAN);
                }

                if (CurrencyManager.Instance != null)
                {
                    CurrencyManager.Instance.AddQuark(dailyQuestRewardQuark);
                }

                QuestManager.Instance.dailyQuestState = DailyQuestState.CompletedToday;
                PlayDialogue(daily_RewardDialogue);
                break;

            case DailyQuestState.CompletedToday:
                PlayDialogue(daily_CompletedDialogue);
                break;
        }
    }

    private void PlayDialogue(DialogueData data)
    {
        if (DialogueManager.Instance != null && data != null)
            DialogueManager.Instance.StartDialogue(data);
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