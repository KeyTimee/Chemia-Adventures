using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory inventory;

    private void Start()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager belum ada. Pastikan mulai game dari scene Bootstrap.");
            return;
        }

        inventory = InventoryManager.Instance.inventory;
    }
}