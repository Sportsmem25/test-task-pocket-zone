using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public Transform slotsParent;
    public GameObject slotPrefab;
    List<GameObject> uiSlots = new List<GameObject>();

    private void Start()
    {
        if (inventory == null)
            inventory = FindObjectOfType<Inventory>();
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (inventory == null || slotsParent == null || slotPrefab == null) return;
        foreach (Transform t in slotsParent) Destroy(t.gameObject);
        uiSlots.Clear();

        foreach (var slot in inventory.slots)
        {
            var item = ItemDatabase.Instance?.Get(slot.itemId);
            if (item == null) continue;
            var prefabToUse = item.uiSlotPrefab != null ? item.uiSlotPrefab : slotPrefab;
            var go = Instantiate(prefabToUse, slotsParent);

            var icon = go.transform.Find("Icon")?.GetComponent<Image>();
            var countText = go.transform.Find("Count")?.GetComponent<Text>();
            var deleteBtnGO = go.transform.Find("DeleteButton")?.GetComponent<Button>();
            var slotButton = go.GetComponent<Button>();

            if (icon != null && item.icon != null)
                icon.sprite = item.icon;

            if (countText != null) 
                countText.text = slot.count > 1 ? slot.count.ToString() : "";

            if (deleteBtnGO != null)
            {
                deleteBtnGO.gameObject.SetActive(false);
                string currentId = slot.itemId;
                deleteBtnGO.onClick.RemoveAllListeners();
                deleteBtnGO.onClick.AddListener(() =>
                {
                    inventory.RemoveItem(currentId, 1);
                    RefreshUI();
                });
            }

            // активировать DeleteButton при клике по слоту предмета
            if (slotButton != null)
            {
                slotButton.onClick.RemoveAllListeners();
                slotButton.onClick.AddListener(() =>
                {
                    // показать/скрыть DeleteButton
                    if (deleteBtnGO != null)
                        deleteBtnGO.gameObject.SetActive(!deleteBtnGO.gameObject.activeSelf);
                });
            }
            uiSlots.Add(go);
        }
    }
}