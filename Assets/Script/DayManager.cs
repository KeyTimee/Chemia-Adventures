using UnityEngine;
using TMPro;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    [Header("Data Hari")]
    public int currentDay = 1;

    [Header("UI Reference")]
    public TextMeshProUGUI dayText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddDays(int daysToAdd)
    {
        if (daysToAdd <= 0) return;

        currentDay += daysToAdd;
        UpdateUI();
        Debug.Log($"Waktu dilewati sebanyak {daysToAdd} hari. Sekarang Hari ke-{currentDay}");

        // Reset Daily Quest setiap kali hari bertambah!
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.ResetDailyQuest();
            QuestManager.Instance.CheckQuest2UnlockCondition();
        }
    }

    public void UpdateUI()
    {
        // Cari otomatis jika reference-nya sempat hilang
        if (dayText == null)
        {
            GameObject dayObj = GameObject.Find("DayText");
            if (dayObj != null) dayText = dayObj.GetComponent<TextMeshProUGUI>();
        }

        if (dayText != null)
        {
            dayText.text = "Day " + currentDay;
        }
    }
}