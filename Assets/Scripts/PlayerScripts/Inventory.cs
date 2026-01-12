using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public string itemId;
    public int count;
}

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> Slots = new List<InventorySlot>();
    public InventoryUI Ui;

    private int capacity = 20;

    private void Start()
    {
        Ui = FindObjectOfType<InventoryUI>();
    }

    public bool AddItem(string itemId, int amount = 1)
    {
        if (string.IsNullOrEmpty(itemId) || amount <= 0) return false;
        Item item = ItemDatabase.Instance?.Get(itemId);
        if (item == null)
        {
            Debug.LogWarning($"Item not found: {itemId}");
            return false;
        }

        InventorySlot slot = Slots.Find(s => s.itemId == itemId);
        if (slot != null)
        {
            int canAdd = Mathf.Min(item.MaxStack - slot.count, amount);
            slot.count += canAdd;
            amount -= canAdd;
        }

        while (amount > 0 && Slots.Count < capacity)
        {
            int add = Mathf.Min(amount, item.MaxStack);
            Slots.Add(new InventorySlot { itemId = itemId, count = add });
            amount -= add;
        }
        Ui?.RefreshUI();
        return true;
    }

    public void RemoveItem(string itemId, int amount = 1)
    {
        InventorySlot slot = Slots.Find(s => s.itemId == itemId);
        if (slot == null) return;
        slot.count -= amount;
        if (slot.count <= 0) Slots.Remove(slot);

        Ui?.RefreshUI();
    }
}