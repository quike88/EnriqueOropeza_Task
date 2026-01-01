using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LootEntry
{
    public ItemData item;
    [Range(0, 100)]
    public float dropChance; 
}

public class LootSpawner : MonoBehaviour
{
    [Header("Configuración de Botín")]
    [SerializeField] private List<LootEntry> lootTable;
    [SerializeField] private GameObject itemPickupPrefab;
    [SerializeField] private float spawnHeightOffset = 0.5f;
    [SerializeField] private float spreadRange = 1.0f;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDeath += SpawnLoot;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDeath -= SpawnLoot;
        }
    }

    private void SpawnLoot()
    {
        foreach (LootEntry entry in lootTable)
        {
            float randomValue = Random.Range(0f, 100f);

            if (randomValue <= entry.dropChance)
            {
                DropItem(entry.item);
            }
        }
    }

    private void DropItem(ItemData item)
    {
        if (itemPickupPrefab == null)
        {
            return;
        }

        Vector3 randomSpread = new Vector3(
            Random.Range(-spreadRange, spreadRange),
            spawnHeightOffset,
            Random.Range(-spreadRange, spreadRange)
        );

        Vector3 spawnPosition = transform.position + randomSpread;

        GameObject droppedItem = Instantiate(itemPickupPrefab, spawnPosition, Quaternion.identity);

        ItemInteractable interactable = droppedItem.GetComponent<ItemInteractable>();
        if (interactable != null)
        {
            interactable.SetItemData(item);
        }
    }
}