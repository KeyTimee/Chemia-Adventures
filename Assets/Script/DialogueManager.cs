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

    // Mengembalikan true selama dialog aktif ATAU baru saja ditutup (< 0.2 detik lalu)
    // sehingga tombol 'E' penutup dialog tidak langsung memicu dialog baru!
    public bool IsDialogueActive => (dialoguePanel != null && dialoguePanel.activeSelf) || (Time.unscaledTime - lastEndTime < 0.2f);

    private Queue<DialogueLine> linesQueue = new Queue<DialogueLine>();
    private Coroutine typingCoroutine;
    private DialogueLine currentLine;
    private bool isTyping = false;
    private Action onDialogueComplete;
    private float lastEndTime = -10f;

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
        // Cek langsung status panel (bukan IsDialogueActive yang ada cooldown-nya)
        if (dialoguePanel == null || !dialoguePanel.activeSelf) return;

        if (Input.GetKeyDown(advanceKey) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(DialogueData data, Action onComplete = null)
    {
        if (data == null || data.lines == null || data.lines.Count == 0) return;

        onDialogueComplete = onComplete;
        linesQueue.Clear();

        foreach (DialogueLine line in data.lines)
        {
            linesQueue.Enqueue(line);
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            ShowAllCharacters();
            isTyping = false;

            if (nextIndicator != null)
                nextIndicator.SetActive(true);
            return;
        }

        if (linesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentLine = linesQueue.Dequeue();

        // Perbarui Nama & Portrait sesuai pembicara di baris ini
        bool hasName = !string.IsNullOrEmpty(currentLine.speakerName);
        if (nameBanner != null) nameBanner.SetActive(hasName);
        if (speakerNameText != null) speakerNameText.text = currentLine.speakerName;
        if (speakerNameTextShadow != null) speakerNameTextShadow.text = currentLine.speakerName;

        bool hasPortrait = currentLine.speakerPortrait != null;
        if (portraitFrame != null) portraitFrame.SetActive(hasPortrait);
        if (portraitImage != null && hasPortrait)
        {
            portraitImage.sprite = currentLine.speakerPortrait;
            portraitImage.preserveAspect = true;
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(currentLine.sentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        if (nextIndicator != null)
            nextIndicator.SetActive(false);

        SetDialogueTexts(sentence);
        SetVisibleCharacters(0);

        // Tunggu 1 frame agar TextMeshPro menghitung jumlah karakter (tanpa tag <sup/sub>)
        yield return null;

        int totalChars = dialogueText != null ? dialogueText.textInfo.characterCount : sentence.Length;

        for (int i = 1; i <= totalChars; i++)
        {
            SetVisibleCharacters(i);
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

    private void SetVisibleCharacters(int count)
    {
        if (dialogueText != null) dialogueText.maxVisibleCharacters = count;
        if (dialogueTextShadow != null) dialogueTextShadow.maxVisibleCharacters = count;
    }

    private void ShowAllCharacters()
    {
        if (dialogueText != null) dialogueText.maxVisibleCharacters = 99999;
        if (dialogueTextShadow != null) dialogueTextShadow.maxVisibleCharacters = 99999;
    }

    private void EndDialogue()
    {
        // Catat waktu penutupan dialog agar NPC tidak langsung re-trigger pada frame yang sama
        lastEndTime = Time.unscaledTime;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        Action callback = onDialogueComplete;
        onDialogueComplete = null;
        callback?.Invoke();
    }
}