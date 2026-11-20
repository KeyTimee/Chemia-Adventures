using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IngredientRequirement
{
    public CollectableType itemType; // Langsung pilih dari dropdown Enum Inventory!
    public Sprite icon;
    public int amountPerCraft = 1;
}

[CreateAssetMenu(fileName = "NewResearchData", menuName = "Chemia/Research Data")]
public class ResearchData : ScriptableObject
{
    [Header("Informasi Proses")]
    public string processID;
    public string processTitle;
    [TextArea(1, 2)]
    public string reactionEquation;
    [TextArea(3, 6)]
    public string description;
    public Sprite diagramSprite;

    [Header("Biaya Riset Awal (Quark)")]
    public int researchCost = 100;

    [Header("Resep Produksi (Per 1x Sintesis)")]
    public List<IngredientRequirement> ingredients = new List<IngredientRequirement>();
    public int quarkCostPerCraft = 0; // Isi 50 untuk Proses Kontak, 0 untuk yang lain
    public Sprite quarkIcon;          // Ikon koin Quark untuk ditampilkan di slot bahan pop-up

    [Header("Hasil Produksi")]
    public CollectableType outputItemType; // Langsung pilih hasil (NAOH, H2SO4, MGOH2)
    public string outputItemName;          // Teks tombol (misal: "NaOH")
}