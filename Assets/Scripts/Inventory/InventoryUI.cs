using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private Image dragIcon;
    [SerializeField] private TooltipUI tooltip;

    [Header("Panels")]
    [SerializeField] private GameObject inventoryContent;

    private List<InventorySlotUI> uiSlots = new List<InventorySlotUI>();
    private int currentlyDraggedIndex = -1;
    private bool isInventoryOpen = false;

    private void Start()
    {
        inventoryManager.OnInventoryUpdated += RefreshUI;

        if (dragIcon != null)
        {
            dragIcon.gameObject.SetActive(false);
            dragIcon.raycastTarget = false;
        }

        InitializeUI();
        CloseInventory();
    }

    private void InitializeUI()
    {
        foreach (Transform child in gridParent) Destroy(child.gameObject);
        uiSlots.Clear();

        var slotsData = inventoryManager.GetSlots();
        for (int i = 0; i < slotsData.Count; i++)
        {
            GameObject obj = Instantiate(slotPrefab, gridParent);
            InventorySlotUI slotUI = obj.GetComponent<InventorySlotUI>();
            uiSlots.Add(slotUI);
            slotUI.Setup(i, this, slotsData[i]);
        }
    }

    public void RefreshUI()
    {
        var slotsData = inventoryManager.GetSlots();
        for (int i = 0; i < uiSlots.Count; i++)
        {
            uiSlots[i].Setup(i, this, slotsData[i]);
        }
    }

    #region Input Callbacks

    public void OnToggleInventory(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (isInventoryOpen)
                CloseInventory();
            else
                OpenInventory();
        }
    }

    #endregion

    private void OpenInventory()
    {
        isInventoryOpen = true;
        inventoryContent.SetActive(true);
    }

    private void CloseInventory()
    {
        isInventoryOpen = false;
        inventoryContent.SetActive(false);
        HideTooltip();
        ClearDraggedItem();
    }

    public void ShowTooltip(ItemData item)
    {
        if (tooltip != null) tooltip.Show(item);
    }

    public void HideTooltip()
    {
        if (tooltip != null) tooltip.Hide();
    }

    public void SetDraggedItem(int index)
    {
        var slots = inventoryManager.GetSlots();
        if (index < 0 || index >= slots.Count || slots[index].item == null) return;

        currentlyDraggedIndex = index;
        dragIcon.sprite = slots[index].item.icon;
        dragIcon.gameObject.SetActive(true);
    }

    public void UpdateDraggedItemPosition(Vector2 position)
    {
        if (dragIcon.gameObject.activeSelf)
        {
            dragIcon.transform.position = position;
        }
    }

    public void ClearDraggedItem()
    {
        currentlyDraggedIndex = -1;
        dragIcon.gameObject.SetActive(false);
    }

    public void RequestSwap(int originIndex, int targetIndex)
    {
        if (originIndex == targetIndex) return;
        inventoryManager.SwapSlots(originIndex, targetIndex);
    }
}