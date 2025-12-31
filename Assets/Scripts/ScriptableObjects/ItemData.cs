using UnityEngine;

public enum ItemType
{
    General,
    Weapon,
    Shield,
    Helmet,
    Chest,
    Pauldrons,
    ElbowPads,
    KneePads,
    Consumable
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea]
    public string description;
    public ItemType itemType;
    public bool isStackable;
}