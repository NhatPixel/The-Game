using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI quantityText;

    InventorySlot inventorySlot;

    static GameObject draggingIcon;
    static InventorySlotUI draggingFromSlot;

    Canvas parentCanvas;

    void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void Set(InventorySlot slot)
    {
        inventorySlot = slot;

        if (slot.IsEmpty)
        {
            icon.enabled = false;
            quantityText.text = "";
        }
        else
        {
            icon.enabled = true;
            icon.sprite = slot.Item.Icon;
            quantityText.text = slot.Quantity > 1 ? slot.Quantity.ToString() : "";
            icon.SetNativeSize();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventorySlot == null || inventorySlot.IsEmpty) return;

        draggingFromSlot = this;

        draggingIcon = new GameObject("DraggingIcon");
        draggingIcon.transform.SetParent(parentCanvas.transform, false);
        draggingIcon.transform.SetAsLastSibling();

        Image img = draggingIcon.AddComponent<Image>();
        img.sprite = inventorySlot.Item.Icon;
        img.raycastTarget = false;
        img.SetNativeSize();

        RectTransform rt = draggingIcon.GetComponent<RectTransform>();
        rt.sizeDelta = icon.rectTransform.sizeDelta;
        rt.pivot = new Vector2(0.5f, 0.5f);
        draggingIcon.transform.position = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggingIcon != null)
        {
            draggingIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggingIcon != null)
        {
            Destroy(draggingIcon);
            draggingIcon = null;
            draggingFromSlot = null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (draggingFromSlot != null && draggingFromSlot != this)
        {
            SwapItems(draggingFromSlot);
        }
    }

    private void SwapItems(InventorySlotUI other)
    {
        var tempItem = inventorySlot.Item;
        var tempQuantity = inventorySlot.Quantity;

        inventorySlot.Item = other.inventorySlot.Item;
        inventorySlot.Quantity = other.inventorySlot.Quantity;

        other.inventorySlot.Item = tempItem;
        other.inventorySlot.Quantity = tempQuantity;

        Set(inventorySlot);
        other.Set(other.inventorySlot);
    }
}
