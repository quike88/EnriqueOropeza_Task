using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    private int slotIndex;
    private InventoryUI inventoryUI;
    private InventorySlotData slotData;
    private bool isDragging = false;
    public void Setup(int index, InventoryUI ui, InventorySlotData data)
    {
        slotIndex = index;
        inventoryUI = ui;
        slotData = data;

        if (slotData != null && slotData.item != null)
        {
            iconImage.sprite = slotData.item.icon;
            iconImage.enabled = true;
            countText.text = slotData.count > 1 ? slotData.count.ToString() : "";
        }
        else
        {
            iconImage.enabled = false;
            countText.text = "";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slotData != null && slotData.item != null && !isDragging)
        {
            inventoryUI.ShowTooltip(slotData.item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryUI.HideTooltip();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotData.item == null) return;

        inventoryUI.HideTooltip(); 
        inventoryUI.SetDraggedItem(slotIndex);
        iconImage.raycastTarget = false;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slotData.item == null) return;
        inventoryUI.UpdateDraggedItemPosition(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        inventoryUI.ClearDraggedItem();
        iconImage.raycastTarget = true;
        isDragging = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlotUI droppedSlot = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (droppedSlot != null)
        {
            inventoryUI.RequestSwap(droppedSlot.slotIndex, this.slotIndex);
        }
    }
}