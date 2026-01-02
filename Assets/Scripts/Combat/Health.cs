using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    public event Action OnDeath;
    public event Action takeDamage;
    public event Action<float, float> OnHealthChanged; // currentHealth, maxHealth

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            takeDamage?.Invoke();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        OnDeath?.Invoke();
    }
    public bool IsDead() => isDead;

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    public float maxHealthValue() => maxHealth;
    public float currentHealthValue() => currentHealth;
}