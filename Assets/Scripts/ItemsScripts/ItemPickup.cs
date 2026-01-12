using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private int amount = 1;
    private string itemID;
    private bool isPickedUp = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPickedUp) return;

        if (collision.gameObject.layer == 3)
        {
            Debug.Log("Столкнулись с игроком");
            Inventory inventory = collision.GetComponent<Inventory>();
            if (inventory != null)
            {
                var item = ItemDatabase.Instance.Get(itemID);
                if (item != null)
                {
                    bool added = inventory.AddItem(itemID, amount);
                    if (added)
                    {
                        isPickedUp = true;
                        inventory.Ui?.RefreshUI();
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