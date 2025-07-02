using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    [SerializeField] private bool isStackable;
    [SerializeField] private int maxStack;
    [TextArea] [SerializeField] private string description;

    public string ItemName => itemName;
    public Sprite Icon => icon;
    public bool IsStackable => isStackable;
    public int MaxStack => maxStack;
    public string Description => description;
}
