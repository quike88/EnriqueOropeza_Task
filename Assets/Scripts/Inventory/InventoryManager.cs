using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int inventorySize = 20;
    [SerializeField] private string saveKey = "PlayerInventorySave";

    [Header("Dependencies")]
    [SerializeField] private CharacterVisualManager visualManager;
    [SerializeField] private Health playerHealth;

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

    private void Awake()
    {
        InitializeInventory();
    }

    private void Start()
    {
        LoadInventory();
        RefreshAllVisuals();
    }

    private void InitializeInventory()
    {
        if (inventorySlots.Count == 0)
        {
            for (int i = 0; i < inventorySize; i++)
            {
                inventorySlots.Add(new InventorySlotData());
            }
        }
    }

    #region Inventory Logic

    public bool AddItem(ItemData item)
    {
        if (item.isStackable)
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.item == item)
                {
                    slot.count++;
                    NotifyUpdate();
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
                NotifyUpdate();
                return true;
            }
        }
        return false;
    }

    public void SwapSlots(InventorySlotData source, InventorySlotData target)
    {
        if (source.item != null && target.item != null && source.item == target.item && source.item.isStackable)
        {
            target.count += source.count;
            source.item = null;
            source.count = 0;
        }
        else
        {
            ItemData tempItem = source.item;
            int tempCount = source.count;

            source.item = target.item;
            source.count = target.count;

            target.item = tempItem;
            target.count = tempCount;
        }

        NotifyUpdate();

        UpdateVisualIfEquipment(source);
        UpdateVisualIfEquipment(target);
    }
    public void RemoveItem(InventorySlotData slot)
    {
        slot.item = null;
        slot.count = 0;
        NotifyUpdate();
        UpdateVisualIfEquipment(slot);
    }
    public void UseQuickSlotItem()
    {
        if (quickSlot.item != null && quickSlot.item.itemType == ItemType.Consumable)
        {
            if (playerHealth != null && !playerHealth.IsDead() && (playerHealth.currentHealthValue() < playerHealth.maxHealthValue()))
            {
                playerHealth.Heal(quickSlot.item.itemValue);
                quickSlot.count--;

                if (quickSlot.count <= 0)
                {
                    quickSlot.item = null;
                }

                NotifyUpdate();
            }
        }
    }
    private void NotifyUpdate()
    {
        OnInventoryUpdated?.Invoke();
        SaveInventory();
    }

    #endregion

    #region Save and Load System

    public void SaveInventory()
    {
        try
        {
            InventorySaveData data = new InventorySaveData();

            foreach (var slot in inventorySlots)
            {
                data.inventorySlots.Add(new SlotSaveData(slot));
            }

            data.weapon = new SlotSaveData(weaponSlot);
            data.shield = new SlotSaveData(shieldSlot);
            data.helmet = new SlotSaveData(helmetSlot);
            data.chest = new SlotSaveData(chestSlot);
            data.pauldrons = new SlotSaveData(pauldronsSlot);
            data.elbowPads = new SlotSaveData(elbowPadsSlot);
            data.kneePads = new SlotSaveData(kneePadsSlot);
            data.quickSlot = new SlotSaveData(quickSlot);

            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(saveKey, json);
            PlayerPrefs.Save();

        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>Inventory System:</color> Error saving: {e.Message}");
        }
    }

    public void LoadInventory()
    {
        if (!PlayerPrefs.HasKey(saveKey))
        {
            return;
        }

        try
        {
            string json = PlayerPrefs.GetString(saveKey);
            InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);

            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (i < data.inventorySlots.Count)
                    ApplySaveDataToSlot(inventorySlots[i], data.inventorySlots[i]);
            }

            ApplySaveDataToSlot(weaponSlot, data.weapon);
            ApplySaveDataToSlot(shieldSlot, data.shield);
            ApplySaveDataToSlot(helmetSlot, data.helmet);
            ApplySaveDataToSlot(chestSlot, data.chest);
            ApplySaveDataToSlot(pauldronsSlot, data.pauldrons);
            ApplySaveDataToSlot(elbowPadsSlot, data.elbowPads);
            ApplySaveDataToSlot(kneePadsSlot, data.kneePads);
            ApplySaveDataToSlot(quickSlot, data.quickSlot);

            OnInventoryUpdated?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"<color=red>Inventory System:</color> Error loading: {e.Message}");
        }
    }

    private void ApplySaveDataToSlot(InventorySlotData slot, SlotSaveData saveData)
    {
        if (saveData == null || string.IsNullOrEmpty(saveData.itemName))
        {
            slot.item = null;
            slot.count = 0;
            return;
        }

        ItemData loadedItem = Resources.Load<ItemData>("Items/" + saveData.itemName);

        if (loadedItem != null)
        {
            slot.item = loadedItem;
            slot.count = saveData.count;
        }
        else
        {
            slot.item = null;
            slot.count = 0;
        }
    }

    #endregion

    #region Visuals and Getters

    private void UpdateVisualIfEquipment(InventorySlotData slot)
    {
        if (visualManager == null) return;

        if (slot == weaponSlot) visualManager.UpdateVisual(ItemType.Weapon, slot.item);
        else if (slot == shieldSlot) visualManager.UpdateVisual(ItemType.Shield, slot.item);
        else if (slot == helmetSlot) visualManager.UpdateVisual(ItemType.Helmet, slot.item);
        else if (slot == chestSlot) visualManager.UpdateVisual(ItemType.Chest, slot.item);
        else if (slot == pauldronsSlot) visualManager.UpdateVisual(ItemType.Pauldrons, slot.item);
        else if (slot == elbowPadsSlot) visualManager.UpdateVisual(ItemType.ElbowPads, slot.item);
        else if (slot == kneePadsSlot) visualManager.UpdateVisual(ItemType.KneePads, slot.item);
    }

    public void RefreshAllVisuals()
    {
        UpdateVisualIfEquipment(weaponSlot);
        UpdateVisualIfEquipment(shieldSlot);
        UpdateVisualIfEquipment(helmetSlot);
        UpdateVisualIfEquipment(chestSlot);
        UpdateVisualIfEquipment(pauldronsSlot);
        UpdateVisualIfEquipment(elbowPadsSlot);
        UpdateVisualIfEquipment(kneePadsSlot);
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

    #endregion
}

[Serializable]
public class InventorySlotData
{
    public ItemData item;
    public int count;
}

[Serializable]
public class SlotSaveData
{
    public string itemName;
    public int count;

    public SlotSaveData(InventorySlotData slot)
    {
        if (slot.item != null)
        {
            itemName = slot.item.name;
            count = slot.count;
        }
        else
        {
            itemName = "";
            count = 0;
        }
    }
}

[Serializable]
public class InventorySaveData
{
    public List<SlotSaveData> inventorySlots = new List<SlotSaveData>();
    public SlotSaveData weapon;
    public SlotSaveData shield;
    public SlotSaveData helmet;
    public SlotSaveData chest;
    public SlotSaveData pauldrons;
    public SlotSaveData elbowPads;
    public SlotSaveData kneePads;
    public SlotSaveData quickSlot;
}