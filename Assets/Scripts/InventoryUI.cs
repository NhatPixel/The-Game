using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    Inventory inventory = new Inventory();
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Transform layout;
    List<InventorySlotUI> inventorySlotUIs = new List<InventorySlotUI>();

    void Start()
    {
        foreach (var slot in inventory.InventorySlots)
        {
            var go = Instantiate(slotPrefab, layout);
            var ui = go.GetComponent<InventorySlotUI>();
            inventorySlotUIs.Add(ui);
        }

        Refresh();
    }

    void OnEnable()
    {
        Refresh();
    }

    void Refresh()
    {
        for (int i = 0; i < inventorySlotUIs.Count; i++)
        {
            inventorySlotUIs[i].Set(inventory.InventorySlots[i]);
        }
    }

    public bool AddItem(Item item)
    {
        bool result = inventory.AddItem(item);
        if (result)
        {
            Refresh();
        }
        return result;
    }
}
