using UnityEngine;

public class SoilInteraction : MonoBehaviour
{
    [Header("UI Reference")]
    public Interact_UI interactUI;
    public GameObject locationIcon;   // Penanda target bracket animasi

    [Header("Visual Soil Point")]
    public SpriteRenderer soilSpriteRenderer;

    private bool isPlayerInRange = false;
    private Player player;

    private void Start()
    {
        if (interactUI != null) interactUI.Hide();
    }

    private void Update()
    {
        UpdateSoilVisibility();

        // 🛑 JIKA QUEST SUDAH SELESAI, JANGAN RESPON TOMBOL 'E' SAMA SEKALI
        if (QuestManager.Instance != null && QuestManager.Instance.quest1State == MainQuestState.Completed)
        {
            if (interactUI != null) interactUI.Hide();
            return;
        }

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            InteractWithSoil();
        }
    }

    private void UpdateSoilVisibility()
    {
        if (QuestManager.Instance == null) return;

        MainQuestState state = QuestManager.Instance.quest1State;

        if (state == MainQuestState.NotStarted)
        {
            if (locationIcon != null) locationIcon.SetActive(false);
        }
        else if (state == MainQuestState.QuestStarted)
        {
            // Quest aktif -> Munculkan icon penanda lokasi sampel tanah
            if (locationIcon != null) locationIcon.SetActive(true);
        }
        else if (state == MainQuestState.SampleCollected)
        {
            // Sampel sudah diambil -> Sembunyikan icon sampai eksperimen selesai
            if (locationIcon != null) locationIcon.SetActive(false);
        }
        else if (state == MainQuestState.ExperimentDone)
        {
            // Eksperimen selesai -> Munculkan icon penanda lokasi penaburan kapur!
            if (locationIcon != null) locationIcon.SetActive(true);
        }
        else if (state == MainQuestState.Completed)
        {
            // Quest selesai -> Matikan icon penanda
            if (locationIcon != null) locationIcon.SetActive(false);
        }
    }

    private void InteractWithSoil()
    {
        if (QuestManager.Instance == null || player == null) return;

        MainQuestState state = QuestManager.Instance.quest1State;

        if (state == MainQuestState.NotStarted)
        {
            Debug.Log("Bicara dengan NPC Perkebunan terlebih dahulu!");
            return;
        }

        switch (state)
        {
            case MainQuestState.QuestStarted:
                // 1. AMBIL SAMPEL TANAH
                player.inventory.Add(CollectableType.SOIL_SAMPLE);
                Debug.Log("Berhasil mengambil Sampel Tanah!");

                QuestManager.Instance.AdvanceQuest1(MainQuestState.SampleCollected);

                if (interactUI != null) interactUI.Hide();
                break;

            case MainQuestState.SampleCollected:
                Debug.Log("Tanah: Sampel sudah diambil. Bawa ke Experiment Table untuk diuji!");
                break;

            case MainQuestState.ExperimentDone:
                // 2. TABUR KAPUR / CACO3 KEMBALI KE TANAH
                if (player.inventory.Contains(CollectableType.LIMESTONEPOWDER))
                {
                    // Hapus 1 unit LIMESTONEPOWDER dari inventory player
                    player.inventory.Remove(CollectableType.LIMESTONEPOWDER);

                    Debug.Log("Berhasil menaburkan Bubuk Kapur (CaCO3) ke tanah perkebunan! Keasaman tanah berhasil dinetralkan.");

                    // Selesaikan Quest 1
                    QuestManager.Instance.AdvanceQuest1(MainQuestState.Completed);

                    if (interactUI != null) interactUI.Hide();
                }
                else
                {
                    Debug.Log("Kamu memerlukan Bubuk Kapur (LIMESTONEPOWDER) di inventory untuk menetralkan tanah ini!");
                }
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (QuestManager.Instance == null) return;

        // 🛑 JIKA QUEST BELUM DIMULAI ATAU SUDAH SELESAI, JANGAN TAMPILKAN UI INTERAKSI
        if (QuestManager.Instance.quest1State == MainQuestState.NotStarted || 
            QuestManager.Instance.quest1State == MainQuestState.Completed)
        {
            return;
        }

        Player p = collision.GetComponent<Player>();

        if (p != null)
        {
            player = p;
            isPlayerInRange = true;

            if (interactUI != null)
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

            if (interactUI != null)
                interactUI.Hide();
        }
    }
}