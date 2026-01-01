using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField] private bool isPlayer = true;
    private Animator animator;
    private PlayerController playerController;
    private EnemyAI enemyAI;
    private CharacterVisualManager characterVisualManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
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