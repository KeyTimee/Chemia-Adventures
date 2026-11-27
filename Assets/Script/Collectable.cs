using UnityEngine;

public class Collectable : MonoBehaviour
{
    public CollectableType type;
    public Sprite icon;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player)
        {
            InventoryManager.Instance.inventory.Add(type);
            Destroy(this.gameObject);
        }
    }
}

public enum CollectableType
{
    NONE,
    WOOD,
    ACID_WATER,
    LIMESTONEPOWDER,
    NEUTRAL_WATER,
    LIMESTONE,
    SOIL_SUSPENSION,
    SOIL_SAMPLE,
    LIMED_SOIL_SUSPENSION,
    WATERING_CAN,
    SEAWATER,          // Larutan Garam (Bahan Klor-Alkali)
    NAOH,              // Natrium Hidroksida (Hasil Klor-Alkali)
    H2SO4,             // Asam Sulfat (Hasil Proses Kontak)
    MGOH2,

    // --- Main Quest 2: Netralisasi Gas Amonia Kandang ---
    GAS_DETECTOR,          // Detektor Gas / Indikator pH Udara (dari John)
    MANURE_SAMPLE,         // Sampel Kotoran Sapi dari kandang Max
    CONCENTRATED_VINEGAR,  // Dirigen Biang Cuka Pekat (dari Villager di Village 2)
    DILUTE_ACETIC_ACID,    // Larutan Asam Asetat Encer (hasil pengenceran di Lab)
    AMMONIA_EXTRACT,       // Ekstrak Larutan Amonia (MANURE_SAMPLE + NEUTRAL_WATER)
    AMMONIUM_ACETATE       // Larutan Amonium Asetat Netral (hasil uji penetralan)
}