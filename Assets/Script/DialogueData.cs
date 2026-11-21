using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Chemia/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("Identitas Pembicara (Kosongkan jika Narasi/Objek)")]
    public string speakerName;
    public Sprite speakerPortrait;

    [Header("Isi Percakapan")]
    [TextArea(2, 5)]
    public List<string> sentences = new List<string>();
}