using UnityEngine;

public class BedInteraction : MonoBehaviour
{
    [Header("UI Interaksi")]
    public GameObject interactText; // Tarik GameObject "Press E to Interact" ke sini

    private bool isPlayerInRange = false;

    private void Start()
    {
        // Pastikan teks interaksi tersembunyi di awal
        if (interactText != null)
        {
            interactText.SetActive(false);
        }
    }

    private void Update()
    {
        // Tekan 'E' saat berada di dekat tempat tidur
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Sembunyikan teks interaksi saat dialog dibuka agar tidak menumpuk
            if (interactText != null)
            {
                interactText.SetActive(false);
            }

            if (BedDialogUI.Instance != null)
            {
                BedDialogUI.Instance.OpenPassTimeDialog();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;

            // Tampilkan teks "Press E to Interact" saat pemain mendekat
            if (interactText != null)
            {
                interactText.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;

            // Sembunyikan kembali teks saat pemain menjauh
            if (interactText != null)
            {
                interactText.SetActive(false);
            }

            // Tutup dialog otomatis jika pemain menjauh dari tempat tidur
            if (BedDialogUI.Instance != null)
            {
                BedDialogUI.Instance.CloseAllDialogs();
            }
        }
    }
}