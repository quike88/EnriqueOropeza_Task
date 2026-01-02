using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private TooltipUI tooltip;
    [SerializeField] private PlayerController playerController;

    [Header("Containers")]
    [SerializeField] private GameObject inventoryContent;
    [SerializeField] private Transform gridParent;

    [Header("Equipment Slots UI")]
    [SerializeField] private InventorySlotUI weaponSlotUI;
    [SerializeField] private InventorySlotUI shieldSlotUI;
    [SerializeField] private InventorySlotUI helmetSlotUI;
    [SerializeField] private InventorySlotUI chestSlotUI;
    [SerializeField] private InventorySlotUI pauldronsSlotUI;
    [SerializeField] private InventorySlotUI elbowPadsUI;
    [SerializeField] private InventorySlotUI kneePadsUI;
    [SerializeField] private InventorySlotUI quickSlotUI;
    [SerializeField] private InventorySlotUI trashSlotUI;

    [Header("Drag Visuals")]
    [SerializeField] private Image dragIcon;
    [Header("Sounds")]
    [SerializeField] private AudioClip openInventorySound;
    [SerializeField] private AudioClip closeInventorySound;
    [SerializeField] private AudioClip dropItemSound;
    [SerializeField] private AudioClip removeSound;

    private List<InventorySlotUI> uiSlots = new List<InventorySlotUI>();
    private bool isInventoryOpen = false;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        inventoryManager.OnInventoryUpdated += RefreshUI;
        if (dragIcon != null) dragIcon.raycastTarget = false;

        InitializeUI();
    }

    private void InitializeUI()
    {
        foreach (Transform child in gridParent) Destroy(child.gameObject);
        uiSlots.Clear();

        var invSlotsData = inventoryManager.GetInventorySlots();
        foreach (var data in invSlotsData)
        {
            GameObject obj = Instantiate(slotPrefab, gridParent);
            InventorySlotUI slotUI = obj.GetComponent<InventorySlotUI>();
            uiSlots.Add(slotUI);
            slotUI.Setup(this, data);
        }
        if (trashSlotUI != null) trashSlotUI.Setup(this, null);
        RefreshUI();
    }

    public void RefreshUI()
    {
        var invSlotsData = inventoryManager.GetInventorySlots();
        for (int i = 0; i < uiSlots.Count; i++)
        {
            uiSlots[i].Setup(this, invSlotsData[i]);
        }

        UpdateSpecialSlot(weaponSlotUI, inventoryManager.GetWeaponSlot());
        UpdateSpecialSlot(shieldSlotUI, inventoryManager.GetShieldSlot());
        UpdateSpecialSlot(helmetSlotUI, inventoryManager.GetHelmetSlot());
        UpdateSpecialSlot(chestSlotUI, inventoryManager.GetChestSlot());
        UpdateSpecialSlot(pauldronsSlotUI, inventoryManager.GetPauldronsSlot());
        UpdateSpecialSlot(elbowPadsUI, inventoryManager.GetElbowPadsSlot());
        UpdateSpecialSlot(kneePadsUI, inventoryManager.GetKneePadsSlot());
        UpdateSpecialSlot(quickSlotUI, inventoryManager.GetQuickSlot());
    }

    private void UpdateSpecialSlot(InventorySlotUI ui, InventorySlotData data)
    {
        if (ui != null) ui.Setup(this, data);
    }
    public void RequestRemove(InventorySlotData data)
    {
        inventoryManager.RemoveItem(data);
        if (removeSound != null)
        {
            AudioManager.Instance.PlaySound(removeSound, Camera.main.transform.position, 0.5f);
        }
    }
    public void OnToggleInventory(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (isInventoryOpen) CloseInventory();
            else OpenInventory();
        }
    }

    private void OpenInventory()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        isInventoryOpen = true;
        inventoryContent.SetActive(true);
        playerController.SetCanMove(false);

        if (openInventorySound != null)
        {
            AudioManager.Instance.PlaySound(openInventorySound, Camera.main.transform.position, 0.5f);
        }
    }

    private void CloseInventory()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        isInventoryOpen = false;
        inventoryContent.SetActive(false);
        playerController.SetCanMove(true);
        tooltip?.Hide();
        dragIcon.gameObject.SetActive(false);

        if (inventoryManager != null) inventoryManager.SaveInventory();
        if (closeInventorySound != null)
        {
            AudioManager.Instance.PlaySound(closeInventorySound, Camera.main.transform.position);
        }
    }

    public void ShowTooltip(ItemData item) => tooltip?.Show(item);
    public void HideTooltip() => tooltip?.Hide();

    public void SetDraggedItem(InventorySlotUI source, InventorySlotData data)
    {
        dragIcon.sprite = data.item.icon;
        dragIcon.gameObject.SetActive(true);
    }

    public void UpdateDraggedItemPosition(Vector2 position)
    {
        if (dragIcon.gameObject.activeSelf) dragIcon.transform.position = position;
    }

    public void ClearDraggedItem() => dragIcon.gameObject.SetActive(false);

    public void RequestSwap(InventorySlotData source, InventorySlotData target)
    {
        inventoryManager.SwapSlots(source, target);
        if (dropItemSound != null)
        {
            AudioManager.Instance.PlaySound(dropItemSound, Camera.main.transform.position, 0.5f);
        }
    }
}