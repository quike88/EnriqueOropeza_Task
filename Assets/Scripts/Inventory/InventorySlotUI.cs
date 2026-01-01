using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Elements")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    [Header("Slot Settings")]
    [SerializeField] private ItemType allowedType = ItemType.General;
    [SerializeField] private bool isSpecialSlot = false;
    [SerializeField] private bool isTrashSlot = false;

    private InventoryUI inventoryUI;
    private InventorySlotData slotData;

    public void Setup(InventoryUI ui, InventorySlotData data)
    {
        inventoryUI = ui;
        slotData = data;
        if (isTrashSlot) return;
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
        if (slotData != null && slotData.item != null)
            inventoryUI.ShowTooltip(slotData.item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryUI.HideTooltip();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotData.item == null) return;

        inventoryUI.HideTooltip();
        inventoryUI.SetDraggedItem(this, slotData);
        iconImage.raycastTarget = false;
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
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlotUI sourceSlot = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (sourceSlot == null) return;
        if (this.isTrashSlot)
        {
            inventoryUI.RequestRemove(sourceSlot.slotData);
            return;
        }

        if (this.isSpecialSlot && sourceSlot.slotData.item != null)
        {
            if (sourceSlot.slotData.item.itemType != this.allowedType) return;
        }

        if (sourceSlot.isSpecialSlot && this.slotData.item != null)
        {
            if (this.slotData.item.itemType != sourceSlot.allowedType) return;
        }
        Debug.Log($"Source Item Type: {sourceSlot.slotData.item.itemType}, Allowed Type: {this.allowedType}");

        inventoryUI.RequestSwap(sourceSlot.slotData, this.slotData);
    }
}