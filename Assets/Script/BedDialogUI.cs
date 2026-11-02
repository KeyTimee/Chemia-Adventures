using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BedDialogUI : MonoBehaviour
{
    public static BedDialogUI Instance { get; private set; }

    [Header("Panel Dialog")]
    public GameObject passTimePanel; // Panel "Pass Time?"
    public GameObject howLongPanel;  // Panel "How Long?"

    [Header("Input Field")]
    public TMP_InputField daysInputField; // Input angka

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        // Sembunyikan panel saat awal
        CloseAllDialogs();
    }

    // Dipanggil saat interaksi dengan tempat tidur
    public void OpenPassTimeDialog()
    {
        CloseAllDialogs();
        if (passTimePanel != null) passTimePanel.SetActive(true);
    }

    // Dynamic Button Event: "Yes" pada Pass Time?
    public void OnClickPassTimeYes()
    {
        if (passTimePanel != null) passTimePanel.SetActive(false);
        if (howLongPanel != null)
        {
            howLongPanel.SetActive(true);
            if (daysInputField != null)
            {
                daysInputField.text = "1"; // Default 1 hari
            }
        }
    }

    // Dynamic Button Event: "No" atau Tombol Close
    public void CloseAllDialogs()
    {
        if (passTimePanel != null) passTimePanel.SetActive(false);
        if (howLongPanel != null) howLongPanel.SetActive(false);
    }

    // Dynamic Button Event: Confirm/Submit pada "How Long?"
    public void OnClickConfirmHowLong()
    {
        if (daysInputField == null) return;

        int inputDays = 1;
        if (int.TryParse(daysInputField.text, out int result))
        {
            inputDays = Mathf.Clamp(result, 1, 3); // Pengaman angka maksimal 3
        }

        // Tambahkan hari melalui DayManager
        if (DayManager.Instance != null)
        {
            DayManager.Instance.AddDays(inputDays);
        }

        CloseAllDialogs();
    }
}