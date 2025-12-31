using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int inventorySize = 20;

    [Header("State")]
    [SerializeField] private List<InventorySlotData> slots = new List<InventorySlotData>();

    public event Action OnInventoryUpdated;

    private void Awake()
    {
        InitializeInventory();
    }

    private void InitializeInventory()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            slots.Add(new InventorySlotData());
        }
    }

    public bool AddItem(ItemData item)
    {
        if (item.isStackable)
        {
            foreach (var slot in slots)
            {
                if (slot.item == item)
                {
                    slot.count++;
                    OnInventoryUpdated?.Invoke();
                    return true;
                }
            }
        }
        foreach (var slot in slots)
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

    public void SwapSlots(int indexA, int indexB)
    {
        InventorySlotData temp = slots[indexA];
        slots[indexA] = slots[indexB];
        slots[indexB] = temp;
        OnInventoryUpdated?.Invoke();
    }

    public List<InventorySlotData> GetSlots() => slots;
}

[Serializable]
public class InventorySlotData
{
    public ItemData item;
    public int count;
}