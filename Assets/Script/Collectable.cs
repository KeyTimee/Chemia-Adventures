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
    WATERING_CAN
}
