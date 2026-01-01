using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;
    private CharacterVisualManager characterVisualManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponentInParent<PlayerController>();
        characterVisualManager = GetComponentInParent<CharacterVisualManager>();
    }

    public void TriggerAttack(int attackIndex)
    {
        animator.SetTrigger("Attack" + attackIndex);
    }

    public void OnAttackFinished()
    {
        if (playerController != null)
        {
            playerController.SetIsAttacking(false);
        }
    }

    public void EnableHitCollider()
    {
        Weapon weapon = characterVisualManager.GetEquippedWeapon();
        if (weapon != null)
        {
            weapon.EnableHitCollider();
        }
    }
    public void DisableHitCollider()
    {
        Weapon weapon = characterVisualManager.GetEquippedWeapon();
        if (weapon != null)
        {
            weapon.DisableHitCollider();
        }
    }
}