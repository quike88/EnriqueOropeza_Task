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

    private void Update()
    {
        // Billboard effect for the prompt
        if (isPlayerInRange && promptRoot != null && promptRoot.activeSelf)
        {
            promptRoot.transform.rotation = Camera.main.transform.rotation;
        }
    }

    public string GetInteractionPrompt()
    {
        return itemData != null ? $"Pick up {itemData.itemName}" : "Pick up item";
    }

    public void Interact()
    {
        if (inventoryManager != null && itemData != null)
        {
            inventoryManager.AddItem(itemData);

            Destroy(gameObject);
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