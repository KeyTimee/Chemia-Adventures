using UnityEngine;

public class ClaraNPC : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject interactText;

    [Header("Dialog Clara (Main Quest 2)")]
    public DialogueData clara_BeforeBriefingJohn; // Jika pemain ke Clara duluan sebelum tanya John
    public DialogueData clara_IntroBarnProblem;   // Penjelasan bau amonia kandang -> suruh cek 4 titik udara
    public DialogueData clara_ReminderCheckAir;   // Pengingat cek 4 titik udara dengan GAS_DETECTOR
    public DialogueData clara_AmmoniaExplain;     // Laporan pH basa -> diskusi ambil sampel kotoran (MANURE_SAMPLE)
    public DialogueData clara_ReminderSample;     // Pengingat ambil MANURE_SAMPLE / bawa ke John
    public DialogueData clara_ReminderNeutralize; // Pengingat semprot larutan asam asetat encer di kandang
    public DialogueData clara_FinalEducation;     // Edukasi kebersihan kandang & pemisahan urine-feses + hadiah

    [Header("Hadiah Main Quest 2")]
    public int mainQuest2RewardQuark = 400;

    private bool isPlayerInRange = false;
    private Player player;

    private void Start()
    {
        if (interactText != null) interactText.SetActive(false);

        // Cek apakah Clara sudah boleh spawn (Quest 1 selesai & sudah berlalu >= 2 hari)
        if (QuestManager.Instance != null)
        {
            bool visible = QuestManager.Instance.ShouldClaraBeVisible();
            gameObject.SetActive(visible);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            return;

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            InteractWithClara();
        }
    }

    private void InteractWithClara()
    {
        if (QuestManager.Instance == null || player == null) return;

        MainQuest2State q2State = QuestManager.Instance.quest2State;

        switch (q2State)
        {
            case MainQuest2State.AvailableAtJohn:
                // Pemain langsung bicara ke Clara sebelum mengambil GAS_DETECTOR dari John
                PlayDialogue(clara_BeforeBriefingJohn);
                break;

            case MainQuest2State.GoToClara:
                QuestManager.Instance.AdvanceQuest2(MainQuest2State.CheckBarnAirPh);
                PlayDialogue(clara_IntroBarnProblem);
                break;

            case MainQuest2State.CheckBarnAirPh:
                PlayDialogue(clara_ReminderCheckAir);
                break;

            case MainQuest2State.ReportAirPhToClara:
                QuestManager.Instance.AdvanceQuest2(MainQuest2State.CollectManureSample);
                PlayDialogue(clara_AmmoniaExplain);
                break;

            case MainQuest2State.CollectManureSample:
            case MainQuest2State.ReportSampleToJohn:
            case MainQuest2State.FindVinegarInVillage2:
            case MainQuest2State.DiluteAndExperiment:
            case MainQuest2State.GetJohnApproval:
                PlayDialogue(clara_ReminderSample);
                break;

            case MainQuest2State.NeutralizeBarnAir:
                PlayDialogue(clara_ReminderNeutralize);
                break;

            case MainQuest2State.ReportFinalToClara:
                // Hapus GAS_DETECTOR jika sudah tidak dipakai (opsional, atau biarkan sebagai kenang-kenangan)
                QuestManager.Instance.AdvanceQuest2(MainQuest2State.Completed);

                if (CurrencyManager.Instance != null)
                {
                    CurrencyManager.Instance.AddQuark(mainQuest2RewardQuark);
                }

                PlayDialogue(clara_FinalEducation);
                break;

            case MainQuest2State.Completed:
                PlayDialogue(clara_FinalEducation);
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