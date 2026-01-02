using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField] private bool isPlayer = true;
    private Animator animator;
    private PlayerController playerController;
    private EnemyAI enemyAI;
    private CharacterVisualManager characterVisualManager;
    private Health health;

    [Header("Sounds")]
    [SerializeField] private AudioClip weaponWhoosh;
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private AudioClip[] takeDamageSounds;
    [SerializeField] private AudioClip[] dieSounds;

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
        if (dieSounds.Length > 0)
        {
            AudioClip clip = dieSounds[Random.Range(0, dieSounds.Length)];
            AudioManager.Instance.PlaySound(clip, transform.position, 1f, Random.Range(0.8f, 1.2f));
        }
        animator.SetTrigger("Die");
        DisableHitCollider();
    }
    private void OnTakeDamage()
    {
        animator.SetTrigger("Hit");
        if (takeDamageSounds.Length > 0)
        {
            AudioClip clip = takeDamageSounds[Random.Range(0, takeDamageSounds.Length)];
            AudioManager.Instance.PlaySound(clip, transform.position, 1f, Random.Range(0.8f, 1.2f));
        }
        if (isPlayer)
        {
            DisableHitCollider();
            playerController.SetIsAttacking(false);
            playerController.SetCanMove(false);
        }
        else
        {
            enemyAI.SetCanMove(false);
            enemyAI.StartChasing();
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
        AudioManager.Instance.PlaySound(weaponWhoosh, transform.position, 1f, Random.Range(0.9f, 1.1f));
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
    public void PlayFootstepSound()
    {
        if (footstepSounds.Length == 0) return;

        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        AudioManager.Instance.PlaySound(clip, transform.position, 0.6f, Random.Range(0.8f, 1.2f));
    }
}