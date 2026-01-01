using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField] private bool isPlayer = true;
    private Animator animator;
    private PlayerController playerController;
    private EnemyAI enemyAI;
    private CharacterVisualManager characterVisualManager;
    private Health health;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        health = GetComponentInParent<Health>();
        if (isPlayer)
        {
            playerController = GetComponentInParent<PlayerController>();
            characterVisualManager = GetComponentInParent<CharacterVisualManager>();
        }
        else
        {
            enemyAI = GetComponentInParent<EnemyAI>();
        }

    }
    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDeath += OnDeath;
            health.takeDamage += OnTakeDamage;
        }
    }
    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDeath -= OnDeath;
            health.takeDamage -= OnTakeDamage;
        }
    }
    private void OnDeath()
    {
        animator.SetTrigger("Die");
        DisableHitCollider();
    }
    private void OnTakeDamage()
    {
        animator.SetTrigger("Hit");
        if (isPlayer)
        {
            DisableHitCollider();
            playerController.SetIsAttacking(false);
            playerController.SetCanMove(false);
        }
        else
        {
            enemyAI.SetCanMove(false);
        }
    }
    public void OnHitAnimationFinished()
    {
        if (isPlayer)
        {
            playerController.SetCanMove(true);
        }
        else
        {
            enemyAI.SetCanMove(true);
        }
        OnAttackFinished();
    }

    public void TriggerAttack(int attackIndex)
    {
        animator.SetBool("IsAttacking", true);
        animator.SetTrigger("Attack" + attackIndex);

    }

    public void OnAttackFinished()
    {
        animator.SetBool("IsAttacking", false);
        if (playerController != null)
        {
            playerController.SetIsAttacking(false);
        }
        else if (enemyAI != null)
        {
            enemyAI.SetIsAttacking(false);
        }
    }

    public void EnableHitCollider()
    {
        Weapon weapon = isPlayer ? characterVisualManager.GetEquippedWeapon() : enemyAI.GetEquippedWeapon();
        if (weapon != null)
        {
            weapon.EnableHitCollider();
        }
    }
    public void DisableHitCollider()
    {
        Weapon weapon = isPlayer ? characterVisualManager.GetEquippedWeapon() : enemyAI.GetEquippedWeapon();
        if (weapon != null)
        {
            weapon.DisableHitCollider();
        }
    }
}