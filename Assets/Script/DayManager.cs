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
        currentDay += daysToAdd;
        UpdateUI();
        Debug.Log($"Waktu dilewati sebanyak {daysToAdd} hari. Sekarang Hari ke-{currentDay}");

        // TODO: Memicu fungsi harian lainnya di sini
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