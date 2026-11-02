using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BedDaySystem : MonoBehaviour
{
    public static BedDaySystem Instance;

    [Header("Status Waktu")]
    public int hariKe = 1;
    public TMP_Text textDisplayHari; // UI opsional untuk menampilkan "Hari ke-X" di layar

    [Header("Panel Konfirmasi (Pass Time)")]
    public GameObject panelPassTime;

    [Header("Panel Input Hari")]
    public GameObject panelInputHari;
    public TMP_InputField inputJumlahHari;
    public TMP_Text textPeringatan; // Teks error jika input tidak valid

    private bool isPlayerDekatKasur = false;

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
        UpdateUIHari();
        
        // Sembunyikan panel UI di awal
        if (panelPassTime != null) panelPassTime.SetActive(false);
        if (panelInputHari != null) panelInputHari.SetActive(false);
    }

    private void Update()
    {
        // Jika pemain dekat kasur dan menekan tombol 'E'
        if (isPlayerDekatKasur && Input.GetKeyDown(KeyCode.E))
        {
            BukaPanelPassTime();
        }
    }

    // --- LOGIKA INTERAKSI KASUR ---

    public void BukaPanelPassTime()
    {
        if (panelPassTime != null) panelPassTime.SetActive(true);
    }

    // Dipanggil oleh Tombol NO di Panel Pass Time
    public void OnTombolNo()
    {
        if (panelPassTime != null) panelPassTime.SetActive(false);
    }

    // Dipanggil oleh Tombol YES di Panel Pass Time
    public void OnTombolYes()
    {
        if (panelPassTime != null) panelPassTime.SetActive(false);
        if (panelInputHari != null) panelInputHari.SetActive(true);
        if (textPeringatan != null) textPeringatan.text = "";
        if (inputJumlahHari != null) inputJumlahHari.text = "1"; // Default 1 hari
    }

    // Dipanggil oleh Tombol CONFIRM di Panel Input Hari
    public void OnTombolKonfirmasiHari()
    {
        if (int.TryParse(inputJumlahHari.text, out int jumlahHari))
        {
            // Validasi input angka 1 - 5
            if (jumlahHari >= 1 && jumlahHari <= 5)
            {
                TambahHari(jumlahHari);
                if (panelInputHari != null) panelInputHari.SetActive(false);
            }
            else
            {
                if (textPeringatan != null) textPeringatan.text = "Masukkan angka 1 sampai 5!";
            }
        }
        else
        {
            if (textPeringatan != null) textPeringatan.text = "Input harus berupa angka!";
        }
    }

    // Dipanggil oleh Tombol BATAL di Panel Input Hari
    public void OnTombolBatalInput()
    {
        if (panelInputHari != null) panelInputHari.SetActive(false);
    }

    private void TambahHari(int jumlah)
    {
        hariKe += jumlah;
        Debug.Log("Pemain tidur selama " + jumlah + " hari. Sekarang Hari ke-" + hariKe);
        UpdateUIHari();

        // Di sini kamu bisa menambahkan efek transisi layar hitam (Fade Out/In) jika ingin
    }

    private void UpdateUIHari()
    {
        if (textDisplayHari != null)
        {
            textDisplayHari.text = "Hari: " + hariKe;
        }
    }

    // --- DETEKSI PLAYER DI DEKAT KASUR ---

    private void OnTriggerEnter2D(Collider2D other) // Gunakan OnTriggerEnter jika 3D
    {
        if (other.CompareTag("Player"))
        {
            isPlayerDekatKasur = true;
            Debug.Log("Tekan 'E' untuk tidur / Pass Time.");
        }
    }

    private void OnTriggerExit2D(Collider2D other) // Gunakan OnTriggerExit jika 3D
    {
        if (other.CompareTag("Player"))
        {
            isPlayerDekatKasur = false;
            OnTombolNo();
            OnTombolBatalInput();
        }
    }
}