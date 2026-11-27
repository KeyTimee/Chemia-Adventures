using UnityEngine;

public class JohnNPC : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject interactText;

    [Header("Dialog John (Main Quest 1)")]
    public DialogueData john_BriefingAwal;        // Babak 1: Suruh temui Max di selatan
    public DialogueData john_ReminderGoToFarm;    // Pengingat pergi ke Farm 1
    public DialogueData john_DiscussSolution;     // Babak 6: Diskusi solusi CaCO3 & lokasi tambang kapur
    public DialogueData john_ReminderFindStone;   // Pengingat cari batu kapur (Timur -> Utara)
    public DialogueData john_StoneSizeLesson;     // Babak 7: Kapur terlalu besar -> suruh grinding & eksperimen
    public DialogueData john_ReminderExperiment;  // Pengingat selesaikan grinding & eksperimen
    public DialogueData john_ApprovalDialogue;    // Babak 8: Persetujuan hasil lab -> tabur ke lahan Max
    public DialogueData john_ReminderNeutralize;  // Pengingat tabur kapur di lahan Max
    public DialogueData john_PostQuestDialogue;   // Cadangan / setelah semua quest selesai

    [Header("Dialog John (Main Quest 2 - Amonia & Cuka)")]
    public DialogueData johnQ2_WaitingForClara;   // Belum 2 hari pasca-Quest 1: belum ada misi baru
    public DialogueData johnQ2_BriefingClara;     // >= 2 hari: info Clara di Farm 1 & beri GAS_DETECTOR
    public DialogueData johnQ2_ReminderGoToClara; // Pengingat temui Clara / cek udara / ambil sampel
    public DialogueData johnQ2_DiscussVinegar;    // Bawa MANURE_SAMPLE: jelaskan NH3 + CH3COOH -> cari cuka di Village 2 (Barat)
    public DialogueData johnQ2_ReminderVinegar;   // Pengingat beli Biang Cuka di Village 2 (arah Barat)
    public DialogueData johnQ2_VinegarBoughtLesson; // Sudah beli CONCENTRATED_VINEGAR -> suruh encerkan & uji di Lab
    public DialogueData johnQ2_ReminderExperiment;// Pengingat selesaikan pengenceran & uji netralisasi di Lab
    public DialogueData johnQ2_ApprovalDialogue;  // Validasi AMMONIUM_ACETATE -> suruh semprotkan larutan cuka encer ke kandang
    public DialogueData johnQ2_ReminderNeutralize;// Pengingat semprotkan larutan di kandang Max
    public DialogueData johnQ2_AllCompleted;      // Setelah Main Quest 2 selesai

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
            InteractWithJohn();
        }
    }

    private void InteractWithJohn()
    {
        if (QuestManager.Instance == null || player == null) return;

        // 1. Jika Main Quest 1 belum selesai, jalankan dialog Main Quest 1
        if (QuestManager.Instance.quest1State != MainQuestState.Completed)
        {
            HandleMainQuest1();
            return;
        }

        // 2. Jika Main Quest 1 sudah selesai, masuk ke alur Main Quest 2
        HandleMainQuest2();
    }

    private void HandleMainQuest1()
    {
        MainQuestState state = QuestManager.Instance.quest1State;

        switch (state)
        {
            case MainQuestState.NotStarted:
                QuestManager.Instance.AdvanceQuest1(MainQuestState.GoToFarm);
                PlayDialogue(john_BriefingAwal);
                break;

            case MainQuestState.GoToFarm:
            case MainQuestState.CheckSoilPh:
            case MainQuestState.ReportPhToMax:
            case MainQuestState.CollectSample:
                PlayDialogue(john_ReminderGoToFarm);
                break;

            case MainQuestState.ReportSampleToJohn:
                QuestManager.Instance.AdvanceQuest1(MainQuestState.FindLimestone);
                PlayDialogue(john_DiscussSolution);
                break;

            case MainQuestState.FindLimestone:
                if (player.inventory.Contains(CollectableType.LIMESTONE))
                {
                    QuestManager.Instance.AdvanceQuest1(MainQuestState.GrindAndExperiment);
                    PlayDialogue(john_StoneSizeLesson);
                }
                else
                {
                    PlayDialogue(john_ReminderFindStone);
                }
                break;

            case MainQuestState.GrindAndExperiment:
                PlayDialogue(john_ReminderExperiment);
                break;

            case MainQuestState.GetJohnApproval:
                QuestManager.Instance.AdvanceQuest1(MainQuestState.NeutralizeSoil);
                PlayDialogue(john_ApprovalDialogue);
                break;

            case MainQuestState.NeutralizeSoil:
            case MainQuestState.ReportFinalToMax:
                PlayDialogue(john_ReminderNeutralize);
                break;

            case MainQuestState.Completed:
                PlayDialogue(john_PostQuestDialogue);
                break;
        }
    }

    private void HandleMainQuest2()
    {
        // Pastikan status timer 2 hari dicek terlebih dahulu
        QuestManager.Instance.CheckQuest2UnlockCondition();
        MainQuest2State q2State = QuestManager.Instance.quest2State;

        switch (q2State)
        {
            case MainQuest2State.Locked:
            case MainQuest2State.WaitingForClara:
                // Belum 2 hari berlalu sejak Quest 1 selesai
                PlayDialogue(johnQ2_WaitingForClara);
                break;

            case MainQuest2State.AvailableAtJohn:
                // Clara sudah spawn di Farm 1! Berikan GAS_DETECTOR & arahkan pemain ke Clara
                if (!player.inventory.Contains(CollectableType.GAS_DETECTOR))
                {
                    player.inventory.Add(CollectableType.GAS_DETECTOR);
                }
                QuestManager.Instance.AdvanceQuest2(MainQuest2State.GoToClara);
                PlayDialogue(johnQ2_BriefingClara);
                break;

            case MainQuest2State.GoToClara:
            case MainQuest2State.CheckBarnAirPh:
            case MainQuest2State.ReportAirPhToClara:
            case MainQuest2State.CollectManureSample:
                PlayDialogue(johnQ2_ReminderGoToClara);
                break;

            case MainQuest2State.ReportSampleToJohn:
                // Pemain datang membawa MANURE_SAMPLE -> John sarankan cari cuka di Village 2 (arah Barat)
                QuestManager.Instance.AdvanceQuest2(MainQuest2State.FindVinegarInVillage2);
                PlayDialogue(johnQ2_DiscussVinegar);
                break;

            case MainQuest2State.FindVinegarInVillage2:
                // Cek apakah pemain sudah membeli Biang Cuka (CONCENTRATED_VINEGAR) dari Village 2
                if (player.inventory.Contains(CollectableType.CONCENTRATED_VINEGAR))
                {
                    QuestManager.Instance.AdvanceQuest2(MainQuest2State.DiluteAndExperiment);
                    PlayDialogue(johnQ2_VinegarBoughtLesson);
                }
                else
                {
                    PlayDialogue(johnQ2_ReminderVinegar);
                }
                break;

            case MainQuest2State.DiluteAndExperiment:
                PlayDialogue(johnQ2_ReminderExperiment);
                break;

            case MainQuest2State.GetJohnApproval:
                QuestManager.Instance.AdvanceQuest2(MainQuest2State.NeutralizeBarnAir);
                PlayDialogue(johnQ2_ApprovalDialogue);
                break;

            case MainQuest2State.NeutralizeBarnAir:
            case MainQuest2State.ReportFinalToClara:
                PlayDialogue(johnQ2_ReminderNeutralize);
                break;

            case MainQuest2State.Completed:
                PlayDialogue(johnQ2_AllCompleted != null ? johnQ2_AllCompleted : john_PostQuestDialogue);
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