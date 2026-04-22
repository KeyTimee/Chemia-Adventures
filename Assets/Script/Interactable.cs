using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Usage")]
    public int maxUses = 1; // -1 = infinite
    private int currentUses;
    public CollectableType type;
    public Sprite icon;

    [Header("UI")]
    public Interact_UI interactUI;

    private bool isPlayerInRange = false;
    private Player player;

    private void Start()
    {
        currentUses = maxUses;

        if (interactUI != null)
            interactUI.Hide();
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    void Interact()
    {
        if (player == null) return;

        player.inventory.Add(type);
        Debug.Log("Ambil: " + type);

        // 🔥 Kurangi penggunaan
        if (maxUses != -1)
        {
            currentUses--;

            if (currentUses <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("MASUK");
        Player p = collision.GetComponent<Player>();

        if (p != null)
        {
            player = p;
            isPlayerInRange = true;

            if (interactUI != null)
                interactUI.Show("Press E");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("KELUAR");
        Player p = collision.GetComponent<Player>();

        if (p != null)
        {
            isPlayerInRange = false;
            player = null;

            if (interactUI != null)
                interactUI.Hide();
        }
    }
}