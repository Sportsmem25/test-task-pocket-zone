using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform slotsParent;
    [SerializeField] private GameObject slotPrefab;

    private Inventory inventory;
    private List<GameObject> uiSlots = new List<GameObject>();

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

        foreach (var slot in inventory.Slots)
        {
            Item item = ItemDatabase.Instance?.Get(slot.itemId);
            if (item == null) continue;
            GameObject prefabToUse = item.UiSlotPrefab != null ? item.UiSlotPrefab : slotPrefab;
            GameObject go = Instantiate(prefabToUse, slotsParent);

            Image icon = go.transform.Find("Icon")?.GetComponent<Image>();
            Text countText = go.transform.Find("Count")?.GetComponent<Text>();
            Button deleteBtnGO = go.transform.Find("DeleteButton")?.GetComponent<Button>();
            Button slotButton = go.GetComponent<Button>();

            if (icon != null && item.Icon != null)
                icon.sprite = item.Icon;

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