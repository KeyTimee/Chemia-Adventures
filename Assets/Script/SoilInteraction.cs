using UnityEngine;

public class SoilInteraction : MonoBehaviour
{
    [Header("Pengaturan Lahan")]
    public string blockID = "Lahan_KiriAtas";
    public bool isSeverelyAcidic = false; // Centang HANYA pada 2 petak kiri!
    public int maxSampleInInventory = 2;

    [Header("UI Reference")]
    public Interact_UI interactUI;
    public GameObject locationIcon;

    [Header("Dialog Tanah")]
    public DialogueData checkPhDialogue;           // Info pH petak ini
    public DialogueData allCheckedPromptDialogue;  // Muncul otomatis setelah petak ke-4 dicek -> lapor ke Max
    public DialogueData sampleCollectedDialogue;   // Sampel terambil -> lapor ke John
    public DialogueData maxSampleReachedDialogue;  // Jika sampel sudah cukup
    public DialogueData oneBlockHealedDialogue;    // Petak kiri pertama ditaburi CaCO3
    public DialogueData allBlocksHealedDialogue;   // Kedua petak kiri ditaburi -> lapor ke Max
    public DialogueData needLimestoneDialogue;     // Jika stok bubuk CaCO3 kurang

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

        if (!isActiveTarget && interactUI != null)
            interactUI.Hide();

        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            return;

        if (isPlayerInRange && isActiveTarget && Input.GetKeyDown(KeyCode.E))
        {
            InteractWithSoil();
        }
    }

    private bool IsTargetActiveNow()
    {
        if (QuestManager.Instance == null) return false;

        switch (QuestManager.Instance.quest1State)
        {
            case MainQuestState.CheckSoilPh:
                return !QuestManager.Instance.IsBlockChecked(blockID);

            case MainQuestState.CollectSample:
            case MainQuestState.ReportSampleToJohn:
                // Tetap aktif di 2 petak kiri agar bisa ambil sampel ulang jika eksperimen gagal
                return isSeverelyAcidic;

            case MainQuestState.NeutralizeSoil:
                return isSeverelyAcidic && !QuestManager.Instance.IsBlockNeutralized(blockID);

            default:
                return false;
        }
    }

    private void InteractWithSoil()
    {
        if (QuestManager.Instance == null || player == null) return;

        MainQuestState state = QuestManager.Instance.quest1State;

        switch (state)
        {
            case MainQuestState.CheckSoilPh:
                QuestManager.Instance.RegisterCheckedBlock(blockID);

                if (DialogueManager.Instance != null && checkPhDialogue != null)
                {
                    DialogueManager.Instance.StartDialogue(checkPhDialogue, () =>
                    {
                        if (QuestManager.Instance.quest1State == MainQuestState.ReportPhToMax &&
                            allCheckedPromptDialogue != null)
                        {
                            DialogueManager.Instance.StartDialogue(allCheckedPromptDialogue);
                        }
                    });
                }
                break;

            case MainQuestState.CollectSample:
            case MainQuestState.ReportSampleToJohn:
                if (!isSeverelyAcidic) return;

                if (player.inventory.GetItemCount(CollectableType.SOIL_SAMPLE) >= maxSampleInInventory)
                {
                    if (DialogueManager.Instance != null && maxSampleReachedDialogue != null)
                        DialogueManager.Instance.StartDialogue(maxSampleReachedDialogue);
                    return;
                }

                player.inventory.Add(CollectableType.SOIL_SAMPLE);

                if (state == MainQuestState.CollectSample)
                {
                    QuestManager.Instance.AdvanceQuest1(MainQuestState.ReportSampleToJohn);
                }

                if (DialogueManager.Instance != null && sampleCollectedDialogue != null)
                    DialogueManager.Instance.StartDialogue(sampleCollectedDialogue);
                break;

            case MainQuestState.NeutralizeSoil:
                if (!isSeverelyAcidic || QuestManager.Instance.IsBlockNeutralized(blockID)) return;

                if (player.inventory.Contains(CollectableType.LIMESTONEPOWDER))
                {
                    player.inventory.Remove(CollectableType.LIMESTONEPOWDER);
                    QuestManager.Instance.RegisterNeutralizedBlock(blockID);

                    bool allDone = (QuestManager.Instance.quest1State == MainQuestState.ReportFinalToMax);
                    DialogueData dlg = allDone ? allBlocksHealedDialogue : oneBlockHealedDialogue;

                    if (DialogueManager.Instance != null && dlg != null)
                        DialogueManager.Instance.StartDialogue(dlg);
                }
                else
                {
                    if (DialogueManager.Instance != null && needLimestoneDialogue != null)
                        DialogueManager.Instance.StartDialogue(needLimestoneDialogue);
                }
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