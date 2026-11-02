using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    [Header("UI Reference (Otomatis jika kosong)")]
    public TextMeshProUGUI quarkText;

    private void Awake()
    {
        // 1. Ambil komponen TextMeshPro langsung dari GameObject ini jika slot Inspector kosong
        if (quarkText == null)
        {
            quarkText = GetComponent<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        // 2. Refresh teks saat game dimulai
        RefreshUI();
    }

    private void OnEnable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnQuarkChanged += UpdateQuarkUI;
            RefreshUI();
        }
    }

    private void OnDisable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnQuarkChanged -= UpdateQuarkUI;
        }
    }

    private void Update()
    {
        // 3. Cadangan: Jika angka di UI belum sama dengan nilai di CurrencyManager, paksa perbarui!
        if (CurrencyManager.Instance != null && quarkText != null)
        {
            string currentVal = CurrencyManager.Instance.CurrentQuark.ToString();
            if (quarkText.text != currentVal)
            {
                quarkText.text = currentVal;
            }
        }
    }

    public void RefreshUI()
    {
        if (CurrencyManager.Instance != null)
        {
            UpdateQuarkUI(CurrencyManager.Instance.CurrentQuark);
        }
    }

    public void UpdateQuarkUI(int amount)
    {
        if (quarkText != null)
        {
            quarkText.text = amount.ToString();
        }
    }
}