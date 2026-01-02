using UnityEngine;

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking
}
[RequireComponent(typeof(CharacterController))]
public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private EnemyState currentState = EnemyState.Idle;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float detectionAngle = 30f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float attackCooldown = 2f;
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Weapon weapon;

    private CharacterController characterController;
    private Health health;
    private Transform playerTransform;
    private bool isAttacking = false;
    private Vector3 verticalVelocity;
    private Quaternion initialRotation;
    private float lastAttackTime = 0f;
    private bool canMove = true;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        initialRotation = transform.rotation;
        health = GetComponent<Health>();
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }
    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDeath += OnDeath;
        }
    }
    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDeath -= OnDeath;
        }
    }
    private void OnDeath()
    {
        canMove = false;
        characterController.height = 0.5f;
        characterController.radius = 0.3f;
        Destroy(this.gameObject, 10f);
    }

    private void Update()
    {
        ApplyGravity();
        if (playerTransform == null || !canMove) return;

        switch (currentState)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;
            case EnemyState.Chasing:
                UpdateChasing();
                break;
            case EnemyState.Attacking:
                UpdateAttacking();
                break;
        }

    }
    private void ApplyGravity()
    {
        if (characterController.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }
        else
        {
            verticalVelocity.y += gravity * Time.deltaTime;
        }

        characterController.Move(verticalVelocity * Time.deltaTime);
    }
    private void UpdateIdle()
    {
        if (animator) animator.SetFloat("Speed", 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, rotationSpeed * Time.deltaTime);

        if (CanSeePlayer())
        {
            currentState = EnemyState.Chasing;
        }
    }

    private void UpdateChasing()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= attackRange)
        {
            currentState = EnemyState.Attacking;
            return;
        }

        if (distance > detectionRange * 1.2f) 
        {
            currentState = EnemyState.Idle;
            return;
        }

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        characterController.Move(direction * moveSpeed * Time.deltaTime);

        if (animator) animator.SetFloat("Speed", 1);
    }

    private void UpdateAttacking()
    {
        if (animator) animator.SetFloat("Speed", 0);

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance > attackRange && !isAttacking)
        {
            currentState = EnemyState.Chasing;
            return;
        }

        if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        isAttacking = true;

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);

        if (animationHandler)
        {
            animationHandler.TriggerAttack(Random.Range(1, 4));
        }
    }

    public void SetIsAttacking(bool state)
    {
        isAttacking = state;
        if (!state)
        {
            lastAttackTime = Time.time;
        }
    }
    public void StartChasing()
    {
        if (currentState == EnemyState.Idle)
        {
            currentState = EnemyState.Chasing;
        }
    }
    public void SetCanMove(bool state)
    {
        canMove = state;
    }
    public Weapon GetEquippedWeapon()
    {
        if (weapon != null)
        {
            return weapon;
        }
        return null;
    }
    private bool CanSeePlayer()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z) + Vector3.up;
        if (distance > detectionRange) return false;

        float[] angles = { 0, -detectionAngle, detectionAngle };

        foreach (float angle in angles)
        {
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, direction, out hit, detectionRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z) + Vector3.up;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Vector3 leftDir = Quaternion.Euler(0, -detectionAngle, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, detectionAngle, 0) * transform.forward;
        Gizmos.DrawRay(rayOrigin, transform.forward * detectionRange);
        Gizmos.DrawRay(rayOrigin, leftDir * detectionRange);
        Gizmos.DrawRay(rayOrigin, rightDir * detectionRange);
    }
}
