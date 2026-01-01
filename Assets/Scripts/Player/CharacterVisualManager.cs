using UnityEngine;
using System.Collections.Generic;

public class CharacterVisualManager : MonoBehaviour
{
    [System.Serializable]
    public class VisualEntry
    {
        public string id;
        public GameObject meshObject;
    }

    [System.Serializable]
    public class VisualGroup
    {
        public ItemType type;
        public List<VisualEntry> entries;
        public Transform socket;
    }
    [Header("Configuration")]
    [SerializeField] private List<VisualGroup> equipmentVisuals;

    private Dictionary<ItemType, GameObject> spawnedItems = new Dictionary<ItemType, GameObject>();

    public void UpdateVisual(ItemType type, ItemData item)
    {
        VisualGroup group = equipmentVisuals.Find(g => g.type == type);
        if (group == null) return;

        if (group.socket != null)
        {
            HandleSocketItem(type, group, item);
        }
        else
        {
            HandleStaticMesh(group, item?.visualID);
        }
    }

    private void HandleSocketItem(ItemType type, VisualGroup group, ItemData item)
    {
        if (spawnedItems.ContainsKey(type) && spawnedItems[type] != null)
        {
            Destroy(spawnedItems[type]);
            spawnedItems.Remove(type);
        }

        if (item == null || item.visualPrefab == null)
        {
            return;
        }

        SpawnItem(type, group, item.visualPrefab, item.itemValue);
    }

    private void SpawnItem(ItemType type, VisualGroup group, GameObject prefab, float damageValue)
    {
        GameObject newVisual = Instantiate(prefab, group.socket);
        Weapon weaponComponent = newVisual.GetComponent<Weapon>();
        if (weaponComponent != null)
        {
            weaponComponent.Setup(damageValue, this.transform.parent.gameObject);
        }
        newVisual.transform.localPosition = Vector3.zero;
        newVisual.transform.localRotation = Quaternion.identity;
        spawnedItems[type] = newVisual;
    }

    private void HandleStaticMesh(VisualGroup group, string visualID)
    {
        if (group.entries.Count == 0) return;

        foreach (var entry in group.entries)
        {
            if (entry.meshObject != null)
                entry.meshObject.SetActive(false);
        }

        bool found = false;
        if (!string.IsNullOrEmpty(visualID))
        {
            VisualEntry target = group.entries.Find(e => e.id == visualID);
            if (target != null && target.meshObject != null)
            {
                target.meshObject.SetActive(true);
                found = true;
            }
        }

        if (!found && group.entries[0].meshObject != null)
        {
            group.entries[0].meshObject.SetActive(true);
        }
    }
    public Weapon GetEquippedWeapon()
    {
        if (spawnedItems.TryGetValue(ItemType.Weapon, out GameObject weapon))
        {
            return weapon.GetComponent<Weapon>();
        }
        return null;
    }
}