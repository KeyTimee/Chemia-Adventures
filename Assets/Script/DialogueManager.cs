using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Panel & Indikator")]
    public GameObject dialoguePanel;
    public GameObject nextIndicator;

    [Header("Nama Pembicara")]
    public GameObject nameBanner;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI speakerNameTextShadow;

    [Header("Portrait Karakter")]
    public GameObject portraitFrame;
    public Image portraitImage;

    [Header("Teks Percakapan")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI dialogueTextShadow;

    [Header("Pengaturan Efek Ketik")]
    public float typingSpeed = 0.025f;
    public KeyCode advanceKey = KeyCode.E;

    public bool IsDialogueActive => dialoguePanel != null && dialoguePanel.activeSelf;

    private Queue<string> sentences = new Queue<string>();
    private Coroutine typingCoroutine;
    private string currentSentence = "";
    private bool isTyping = false;
    private Action onDialogueComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!IsDialogueActive) return;

        // Lanjut kalimat dengan tombol E, Spasi, atau Klik Kiri Mouse
        if (Input.GetKeyDown(advanceKey) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(DialogueData data, Action onComplete = null)
    {
        if (data == null || data.sentences == null || data.sentences.Count == 0) return;

        onDialogueComplete = onComplete;

        // 1. Atur Nama Pembicara (sembunyikan banner jika kosong)
        bool hasName = !string.IsNullOrEmpty(data.speakerName);
        if (nameBanner != null) nameBanner.SetActive(hasName);
        if (speakerNameText != null) speakerNameText.text = data.speakerName;
        if (speakerNameTextShadow != null) speakerNameTextShadow.text = data.speakerName;

        // 2. Atur Portrait Karakter (sembunyikan bingkai jika tidak ada sprite)
        bool hasPortrait = data.speakerPortrait != null;
        if (portraitFrame != null) portraitFrame.SetActive(hasPortrait);
        if (portraitImage != null && hasPortrait)
        {
            portraitImage.sprite = data.speakerPortrait;
            portraitImage.preserveAspect = true;
        }

        // 3. Masukkan semua kalimat ke antrean
        sentences.Clear();
        foreach (string sentence in data.sentences)
        {
            sentences.Enqueue(sentence);
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        // Jika sedang mengetik, langsung selesaikan kalimat saat ini
        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            SetDialogueTexts(currentSentence);
            isTyping = false;

            if (nextIndicator != null)
                nextIndicator.SetActive(true);
            return;
        }

        // Jika kalimat habis, tutup dialog
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentSentence = sentences.Dequeue();
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        if (nextIndicator != null)
            nextIndicator.SetActive(false);

        SetDialogueTexts("");

        foreach (char letter in sentence.ToCharArray())
        {
            string updated = dialogueText.text + letter;
            SetDialogueTexts(updated);
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        if (nextIndicator != null)
            nextIndicator.SetActive(true);
    }

    private void SetDialogueTexts(string textValue)
    {
        if (dialogueText != null) dialogueText.text = textValue;
        if (dialogueTextShadow != null) dialogueTextShadow.text = textValue;
    }

    private void EndDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        Action callback = onDialogueComplete;
        onDialogueComplete = null;
        callback?.Invoke();
    }
}