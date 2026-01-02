using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 1.2f;
    private bool isJumpPressed = false;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private CinemachineInputAxisController cameraInputController;

    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector3 verticalVelocity;
    private Transform cameraTransform;

    private bool isAttacking = false;
    private int currentAttack = 0;
    private bool canMove = true;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        ApplyGravity();
        if (!isAttacking && canMove)
        {
            HandleMovement();
        }
    }

    #region Input Callbacks

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PerformInteraction();
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking && canMove && inventoryManager.GetWeaponSlot().item != null)
        {
            StartAttack();
        }
    }
    public void OnUseItem (InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            inventoryManager.UseQuickSlotItem();
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && characterController.isGrounded && !isAttacking && canMove)
        {
            //isJumpPressed = true;
        }
    }
    #endregion

    private void HandleMovement()
    {
        if (cameraTransform == null) return;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        if (moveDirection.magnitude > 0.1f)
        {
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", moveDirection.magnitude, 0.1f, Time.deltaTime);
        }
    }

    private void PerformInteraction()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange, interactableLayer);
        
        foreach (var collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
                break;
            }
        }
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }

        if (isJumpPressed)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumpPressed = false;

            if (animator != null) animator.SetTrigger("Jump");
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        characterController.Move(verticalVelocity * Time.deltaTime);
        animator.SetBool("IsGrounded", characterController.isGrounded);
    }
    private void StartAttack()
    {
        isAttacking = true;

        currentAttack = Random.Range(1, 4);
        if (animationHandler != null)
        {
            animationHandler.TriggerAttack(currentAttack);
        }
    }
    public void SetIsAttacking(bool state)
    {
        isAttacking = state;

        if (!state && moveInput.magnitude < 0.1f && animator != null)
        {
            animator.SetFloat("Speed", 0);
        }
    }
    public void SetCanMove(bool state)
    {
        canMove = state;
        if (cameraInputController != null)
        {
            cameraInputController.enabled = state;
        }

        if (!state)
        {
            animator.SetFloat("Speed", 0);
            moveInput = Vector2.zero;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}