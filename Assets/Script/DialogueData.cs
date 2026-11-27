using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public Sprite speakerPortrait;
    [TextArea(2, 5)]
    public string sentence;
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Chemia/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("Daftar Baris Percakapan (Bisa Berganti Pembicara)")]
    public List<DialogueLine> lines = new List<DialogueLine>();
}