using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemID;
    public int amount = 1;
    private bool isPickedUp = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPickedUp) return;

        if (collision.CompareTag("Player"))
        {
            var inventory = collision.GetComponent<Inventory>();
            if (inventory != null)
            {
                var item = ItemDatabase.Instance.Get(itemID);
                if (item != null)
                {
                    bool added = inventory.AddItem(itemID, amount);
                    if (added)
                    {
                        isPickedUp = true;
                        inventory.ui?.RefreshUI();
                        Destroy(gameObject);
                    }
                    else
                    {
                        Debug.Log("Inventory full, cannot pick up " + itemID);
                    }
                }
            }
        }
    }
}