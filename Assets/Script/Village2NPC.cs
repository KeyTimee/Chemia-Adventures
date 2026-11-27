using UnityEngine;

public class Village2NPC : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject interactText;

    [Header("Pengaturan Villager")]
    public bool isVinegarSeller = false; // Centang HANYA pada 1 villager penjual Biang Cuka!
    public int vinegarPrice = 150;

    [Header("Dialog Villager Biasa (3 NPC Pengalih)")]
    public DialogueData citizen_NormalDialogue;     // Obrolan sehari-hari di Village 2
    public DialogueData citizen_QuestHintDialogue;  // Saat Quest 2 aktif: mengarahkan ke penjual cuka

    [Header("Dialog Penjual Biang Cuka (1 NPC Utama)")]
    public DialogueData seller_BeforeQuestDialogue; // Sebelum disuruh John cari cuka
    public DialogueData seller_NotEnoughQuark;      // Quark < 150 (sarankan kerja harian di Max dulu)
    public DialogueData seller_BuySuccessDialogue;  // Quark >= 150 -> bayar & dapat CONCENTRATED_VINEGAR
    public DialogueData seller_AlreadyBought;       // Sudah membeli Biang Cuka

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
            InteractWithVillager();
        }
    }

    private void InteractWithVillager()
    {
        if (player == null) return;

        bool isLookingForVinegar = QuestManager.Instance != null &&
                                   QuestManager.Instance.quest2State == MainQuest2State.FindVinegarInVillage2;

        // ==========================================
        // 1. JIKA NPC INI ADALAH PENJUAL BIANG CUKA
        // ==========================================
        if (isVinegarSeller)
        {
            if (QuestManager.Instance == null || QuestManager.Instance.quest2State < MainQuest2State.FindVinegarInVillage2)
            {
                PlayDialogue(seller_BeforeQuestDialogue);
                return;
            }

            // Cek jika sudah punya Biang Cuka atau quest sudah lewat tahap beli
            if (player.inventory.Contains(CollectableType.CONCENTRATED_VINEGAR) ||
                QuestManager.Instance.quest2State > MainQuest2State.FindVinegarInVillage2)
            {
                PlayDialogue(seller_AlreadyBought);
                return;
            }

            // Cek apakah Quark pemain mencukupi (>= 150 Quark)
            if (CurrencyManager.Instance != null && CurrencyManager.Instance.SpendQuark(vinegarPrice))
            {
                player.inventory.Add(CollectableType.CONCENTRATED_VINEGAR);
                PlayDialogue(seller_BuySuccessDialogue);
            }
            else
            {
                PlayDialogue(seller_NotEnoughQuark);
            }
            return;
        }

        // ==========================================
        // 2. JIKA NPC INI ADALAH 3 VILLAGER BIASA
        // ==========================================
        if (isLookingForVinegar && citizen_QuestHintDialogue != null)
        {
            PlayDialogue(citizen_QuestHintDialogue);
        }
        else
        {
            PlayDialogue(citizen_NormalDialogue);
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