using UnityEngine;
using System.Collections.Generic;

public class Inventory
{
    int maxSlots = 40;
    List<InventorySlot> inventorySlots = new List<InventorySlot>();
    public List<InventorySlot> InventorySlots => inventorySlots;

    public Inventory()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
    }

    public bool AddItem(Item item, int amount = 1)
    {
        if (item.IsStackable)
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.Item == item && slot.Quantity < item.MaxStack)
                {
                    int addQuantity = Mathf.Min(amount, item.MaxStack - slot.Quantity);
                    slot.Quantity += addQuantity;
                    amount -= addQuantity;

                    if (amount == 0)
                    {
                        return true;
                    }
                }
            }

            foreach (var slot in inventorySlots)
            {
                if (slot.IsEmpty)
                {
                    slot.Item = item;
                    if (amount > item.MaxStack)
                    {
                        slot.Quantity = item.MaxStack;
                        amount -= item.MaxStack;
                    }
                    else
                    {
                        slot.Quantity = amount;
                        amount = 0;
                    }

                    if (amount == 0)
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.IsEmpty)
                {
                    slot.Item = item;
                    slot.Quantity = 1;

                    return true;
                }
            }
        }

        return false;
    }
}
