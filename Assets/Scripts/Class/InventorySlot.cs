using UnityEngine;

public class InventorySlot
{
    Item item;
    int quantity;
    public Item Item { get => item; set => item = value; }
    public int Quantity { get => quantity; set => quantity = value; }
    public bool IsEmpty => item == null;
}
