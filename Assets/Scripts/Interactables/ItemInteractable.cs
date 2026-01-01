using UnityEngine;
using TMPro;

public class ItemInteractable : MonoBehaviour, IInteractable
{
    [Header("Item Data")]
    [SerializeField] private ItemData itemData;

    [Header("UI References - Prompt")]
    [SerializeField] private GameObject promptRoot;
    [SerializeField] private TextMeshProUGUI promptText;

    private InventoryManager inventoryManager;
    private bool isPlayerInRange = false;

    private void Start()
    {
        inventoryManager = Object.FindFirstObjectByType<InventoryManager>();

        if (promptRoot) promptRoot.SetActive(false);

        if (promptText != null && itemData != null)
        {
            promptText.text = GetInteractionPrompt();
        }
    }
    public void SetItemData(ItemData newItemData)
    {
        itemData = newItemData;
        if (promptText != null && itemData != null)
        {
            promptText.text = GetInteractionPrompt();
        }
    }

    private void Update()
    {
        if (isPlayerInRange && promptRoot != null && promptRoot.activeSelf)
        {
            promptRoot.transform.rotation = Camera.main.transform.rotation;
        }
    }

    public string GetInteractionPrompt()
    {
        return  $"Press E to pick up {itemData.itemName}";
    }

    public void Interact()
    {
        if (inventoryManager != null && itemData != null)
        {
            if (inventoryManager.AddItem(itemData))
            {
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Inventory Full! Cannot pick up item.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (promptRoot) promptRoot.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (promptRoot) promptRoot.SetActive(false);
        }
    }
}