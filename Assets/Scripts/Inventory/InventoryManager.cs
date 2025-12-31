using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Temporary list to store collected items
    [SerializeField] private List<ItemData> collectedItems = new List<ItemData>();

    public void AddItem(ItemData item)
    {
        collectedItems.Add(item);
        Debug.Log($"Item added to inventory: {item.itemName}. Total items: {collectedItems.Count}");
    }

    public List<ItemData> GetItems()
    {
        return collectedItems;
    }
}