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
    public int capacity = 20;
    public List<InventorySlot> slots = new List<InventorySlot>();
    public InventoryUI ui;

    private void Start()
    {
        ui = FindObjectOfType<InventoryUI>();
    }

    public bool AddItem(string itemId, int amount = 1)
    {
        if (string.IsNullOrEmpty(itemId) || amount <= 0) return false;
        var item = ItemDatabase.Instance?.Get(itemId);
        if (item == null)
        {
            Debug.LogWarning($"Item not found: {itemId}");
            return false;
        }

        var slot = slots.Find(s => s.itemId == itemId);
        if (slot != null)
        {
            int canAdd = Mathf.Min(item.maxStack - slot.count, amount);
            slot.count += canAdd;
            amount -= canAdd;
        }

        while (amount > 0 && slots.Count < capacity)
        {
            int add = Mathf.Min(amount, item.maxStack);
            slots.Add(new InventorySlot { itemId = itemId, count = add });
            amount -= add;
        }
        ui?.RefreshUI();
        return true;
    }

    public void RemoveItem(string itemId, int amount = 1)
    {
        var slot = slots.Find(s => s.itemId == itemId);
        if (slot == null) return;
        slot.count -= amount;
        if (slot.count <= 0) slots.Remove(slot);

        ui?.RefreshUI();
    }

    //public void UseItem(string itemId, GameObject user)
    //{
    //    var item = ItemDatabase.Instance?.Get(itemId);
    //    if (item == null)
    //    {
    //        Debug.LogWarning("UseItem: item not found: " + itemId);
    //        return;
    //    }
    //    item.Use(user);
    //    RemoveItem(itemId, 1);
    //}
}