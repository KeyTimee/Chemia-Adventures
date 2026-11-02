using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public Inventory inventory;

    [Header("Inventory Settings")]
    public int inventorySize = 21;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Tambahkan baris ini agar ia keluar dari parent sebelum DontDestroyOnLoad
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        if (inventory == null)
        {
            inventory = new Inventory(inventorySize);
        }
    }
}