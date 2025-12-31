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

    [Header("Values")]
    public float itemValue; // Used for Damage, Defense, or Healing points

    [Header("Visual Settings")]
    public string visualID; // Used for static mesh swapping (Armor)
    public GameObject visualPrefab; // Used for instantiation (Weapons/Shields)
}