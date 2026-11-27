using UnityEngine;

public class BarnAirInteraction : MonoBehaviour
{
    [Header("Pengaturan Titik Kandang")]
    public string pointID = "Kandang_Kotoran1";
    public bool isHighAmmoniaSource = false; // Centang HANYA pada 2 titik dekat tumpukan kotoran!
    public int maxManureSampleInInventory = 2;

    [Header("UI Reference")]
    public Interact_UI interactUI;
    public GameObject locationIcon;

    [Header("Dialog Kandang Sapi (Main Quest 2)")]
    public DialogueData checkAirDialogue;          // Info hasil bacaan Gas Detector (pH > 9 basa / pH 7.0 normal)
    public DialogueData allCheckedPromptDialogue;  // Muncul otomatis setelah titik ke-4 dicek -> lapor ke Clara
    public DialogueData needGasDetectorDialogue;   // Jika pemain belum membawa GAS_DETECTOR
    public DialogueData sampleCollectedDialogue;   // Sampel kotoran terambil -> lapor ke John
    public DialogueData maxSampleReachedDialogue;  // Jika stok MANURE_SAMPLE sudah cukup
    public DialogueData onePointHealedDialogue;    // Titik kotoran pertama disemprot DILUTE_ACETIC_ACID
    public DialogueData allPointsHealedDialogue;   // Kedua titik kotoran selesai disemprot -> lapor ke Clara
    public DialogueData needDiluteAcidDialogue;    // Jika stok DILUTE_ACETIC_ACID kurang

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
            InteractWithBarnPoint();
        }
    }

    private bool IsTargetActiveNow()
    {
        if (QuestManager.Instance == null) return false;

        switch (QuestManager.Instance.quest2State)
        {
            case MainQuest2State.CheckBarnAirPh:
                return !QuestManager.Instance.IsAirPointChecked(pointID);

            case MainQuest2State.CollectManureSample:
            case MainQuest2State.ReportSampleToJohn:
                // Tetap aktif di 2 titik kotoran agar pemain bisa ambil sampel ulang jika eksperimen gagal
                return isHighAmmoniaSource;

            case MainQuest2State.NeutralizeBarnAir:
                return isHighAmmoniaSource && !QuestManager.Instance.IsAirPointNeutralized(pointID);

            default:
                return false;
        }
    }

    private void InteractWithBarnPoint()
    {
        if (QuestManager.Instance == null || player == null) return;

        MainQuest2State q2State = QuestManager.Instance.quest2State;

        switch (q2State)
        {
            case MainQuest2State.CheckBarnAirPh:
                // Pastikan pemain membawa GAS_DETECTOR dari John
                if (!player.inventory.Contains(CollectableType.GAS_DETECTOR))
                {
                    if (DialogueManager.Instance != null && needGasDetectorDialogue != null)
                        DialogueManager.Instance.StartDialogue(needGasDetectorDialogue);
                    return;
                }

                QuestManager.Instance.RegisterCheckedAirPoint(pointID);

                if (DialogueManager.Instance != null && checkAirDialogue != null)
                {
                    DialogueManager.Instance.StartDialogue(checkAirDialogue, () =>
                    {
                        if (QuestManager.Instance.quest2State == MainQuest2State.ReportAirPhToClara &&
                            allCheckedPromptDialogue != null)
                        {
                            DialogueManager.Instance.StartDialogue(allCheckedPromptDialogue);
                        }
                    });
                }
                break;

            case MainQuest2State.CollectManureSample:
            case MainQuest2State.ReportSampleToJohn:
                if (!isHighAmmoniaSource) return;

                if (player.inventory.GetItemCount(CollectableType.MANURE_SAMPLE) >= maxManureSampleInInventory)
                {
                    if (DialogueManager.Instance != null && maxSampleReachedDialogue != null)
                        DialogueManager.Instance.StartDialogue(maxSampleReachedDialogue);
                    return;
                }

                player.inventory.Add(CollectableType.MANURE_SAMPLE);

                if (q2State == MainQuest2State.CollectManureSample)
                {
                    QuestManager.Instance.AdvanceQuest2(MainQuest2State.ReportSampleToJohn);
                }

                if (DialogueManager.Instance != null && sampleCollectedDialogue != null)
                    DialogueManager.Instance.StartDialogue(sampleCollectedDialogue);
                break;

            case MainQuest2State.NeutralizeBarnAir:
                if (!isHighAmmoniaSource || QuestManager.Instance.IsAirPointNeutralized(pointID)) return;

                if (player.inventory.Contains(CollectableType.DILUTE_ACETIC_ACID))
                {
                    player.inventory.Remove(CollectableType.DILUTE_ACETIC_ACID);
                    QuestManager.Instance.RegisterNeutralizedAirPoint(pointID);

                    bool allDone = (QuestManager.Instance.quest2State == MainQuest2State.ReportFinalToClara);
                    DialogueData dlg = allDone ? allPointsHealedDialogue : onePointHealedDialogue;

                    if (DialogueManager.Instance != null && dlg != null)
                        DialogueManager.Instance.StartDialogue(dlg);
                }
                else
                {
                    if (DialogueManager.Instance != null && needDiluteAcidDialogue != null)
                        DialogueManager.Instance.StartDialogue(needDiluteAcidDialogue);
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