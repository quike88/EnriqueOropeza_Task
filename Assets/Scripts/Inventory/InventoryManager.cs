using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int inventorySize = 20;

    [Header("Inventory State")]
    [SerializeField] private List<InventorySlotData> inventorySlots = new List<InventorySlotData>();

    [Header("Equipment Slots")]
    [SerializeField] private InventorySlotData weaponSlot = new InventorySlotData();
    [SerializeField] private InventorySlotData shieldSlot = new InventorySlotData();
    [SerializeField] private InventorySlotData helmetSlot = new InventorySlotData();
    [SerializeField] private InventorySlotData chestSlot = new InventorySlotData();
    [SerializeField] private InventorySlotData pauldronsSlot = new InventorySlotData();
    [SerializeField] private InventorySlotData elbowPadsSlot = new InventorySlotData();
    [SerializeField] private InventorySlotData kneePadsSlot = new InventorySlotData();
    [SerializeField] private InventorySlotData quickSlot = new InventorySlotData();

    public event Action OnInventoryUpdated;

    private void Awake() => InitializeInventory();

    private void InitializeInventory()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            inventorySlots.Add(new InventorySlotData());
        }
    }

    public bool AddItem(ItemData item)
    {
        if (item.isStackable)
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.item == item)
                {
                    slot.count++;
                    OnInventoryUpdated?.Invoke();
                    return true;
                }
            }
        }

        foreach (var slot in inventorySlots)
        {
            if (slot.item == null)
            {
                slot.item = item;
                slot.count = 1;
                OnInventoryUpdated?.Invoke();
                return true;
            }
        }
        return false;
    }

    public void SwapSlots(InventorySlotData source, InventorySlotData target)
    {
        ItemData tempItem = source.item;
        int tempCount = source.count;

        source.item = target.item;
        source.count = target.count;

        target.item = tempItem;
        target.count = tempCount;

        OnInventoryUpdated?.Invoke();
    }

    public List<InventorySlotData> GetInventorySlots() => inventorySlots;
    public InventorySlotData GetWeaponSlot() => weaponSlot;
    public InventorySlotData GetShieldSlot() => shieldSlot;
    public InventorySlotData GetHelmetSlot() => helmetSlot;
    public InventorySlotData GetChestSlot() => chestSlot;
    public InventorySlotData GetPauldronsSlot() => pauldronsSlot;
    public InventorySlotData GetElbowPadsSlot() => elbowPadsSlot;
    public InventorySlotData GetKneePadsSlot() => kneePadsSlot;
    public InventorySlotData GetQuickSlot() => quickSlot;
}

[Serializable]
public class InventorySlotData
{
    public ItemData item;
    public int count;
}