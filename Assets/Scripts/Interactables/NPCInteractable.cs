using UnityEngine;
using TMPro;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [Header("NPC Data")]
    [SerializeField] private string npcName;
    [TextArea(3, 10)]
    [SerializeField] private string[] dialogueLines;

    [Header("UI References - Prompt")]
    [SerializeField] private string interactionPrompt = "Press E to talk";
    [SerializeField] private GameObject promptRoot;
    [SerializeField] private TextMeshProUGUI promptText;

    [Header("UI References - Dialogue")]
    [SerializeField] private GameObject dialogueRoot;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI nameText;
    [Header("Sounds")]
    [SerializeField] private AudioClip dialogueSound;

    [Header("Reward Settings")]
    [SerializeField] private GameObject itemPickupPrefab;
    [SerializeField] private ItemData rewardItem;
    [SerializeField] private bool giveRewardOnce = true;
    [SerializeField] private Transform spawnPoint;

    private int currentLineIndex = 0;
    private bool isTalking = false;
    private bool isPlayerInRange = false;
    private bool hasGivenReward = false;

    private void Start()
    {
        if (promptRoot) promptRoot.SetActive(false);
        if (dialogueRoot) dialogueRoot.SetActive(false);

        if (nameText) nameText.text = npcName;

        if (promptText != null)
        {
            promptText.text = GetInteractionPrompt();
        }
    }

    private void Update()
    {
        // Billboarding para el prompt y el diálogo
        if (isPlayerInRange && promptRoot != null && promptRoot.activeSelf)
        {
            promptRoot.transform.rotation = Camera.main.transform.rotation;
        }

        if (isTalking && dialogueRoot != null && dialogueRoot.activeSelf)
        {
            dialogueRoot.transform.rotation = Camera.main.transform.rotation;
        }
    }

    public string GetInteractionPrompt()
    {
        return interactionPrompt;
    }

    public void Interact()
    {
        if (!isTalking)
        {
            StartDialogue();
        }
        else
        {
            DisplayNextLine();
        }
    }

    private void StartDialogue()
    {
        isTalking = true;
        currentLineIndex = 0;

        if (promptRoot) promptRoot.SetActive(false);
        if (dialogueRoot) dialogueRoot.SetActive(true);

        UpdateDialogueText();
    }

    private void DisplayNextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < dialogueLines.Length)
        {
            UpdateDialogueText();
        }
        else
        {
            EndDialogue();
        }
    }

    private void UpdateDialogueText()
    {
        if (dialogueText != null)
        {
            if (dialogueSound != null)
            {
                AudioManager.Instance.PlaySound(dialogueSound, transform.position);
            }
            dialogueText.text = dialogueLines[currentLineIndex];
        }
    }

    private void EndDialogue()
    {
        isTalking = false;
        if (dialogueRoot) dialogueRoot.SetActive(false);

        if (isPlayerInRange && promptRoot) promptRoot.SetActive(true);

        // Lógica de spawn de recompensa
        HandleReward();
    }

    private void HandleReward()
    {
        if (rewardItem != null && itemPickupPrefab != null)
        {
            if (!giveRewardOnce || !hasGivenReward)
            {
                SpawnReward();
                hasGivenReward = true;
            }
        }
    }

    private void SpawnReward()
    {
        GameObject droppedItem = Instantiate(itemPickupPrefab, spawnPoint.position, Quaternion.identity);

        ItemInteractable interactable = droppedItem.GetComponent<ItemInteractable>();
        if (interactable != null)
        {
            interactable.SetItemData(rewardItem);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (!isTalking && promptRoot) promptRoot.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (promptRoot) promptRoot.SetActive(false);

            if (isTalking) EndDialogue();
        }
    }
}