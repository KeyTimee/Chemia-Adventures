using UnityEngine;
using TMPro;

public class Interact_UI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI text;

    private void Start()
    {
        Hide(); // 🔥 pastikan mati saat game mulai
    }

    public void Show(string message)
    {
        if (panel != null)
            panel.SetActive(true);

        if (text != null)
            text.text = message;
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}