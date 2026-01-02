using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private BoxCollider hitCollider;
    [SerializeField] private float damage = 25f;
    [SerializeField] private GameObject owner;

    [Header("Hit effects")]
    [SerializeField] private AudioClip[] hitSounds;
    [SerializeField] private GameObject hitVFXPrefab;

    // to prevent multiple damage applications to the same entity in one hit
    private List<IDamageable> damagedEntities = new List<IDamageable>();

    public void Setup(float damageValue, GameObject weaponOwner)
    {
        this.damage = damageValue;
        this.owner = weaponOwner;
        DisableHitCollider();
    }
    public void EnableHitCollider()
    {
        damagedEntities.Clear();
        if (hitCollider != null)
        {
            hitCollider.enabled = true;
        }
    }
    public void DisableHitCollider()
    {
        if (hitCollider != null)
        {
            hitCollider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && !damagedEntities.Contains(damageable))
        {
            AudioManager.Instance.PlaySound(hitSounds[Random.Range(0, hitSounds.Length)], other.ClosestPoint(transform.position));
            if (hitVFXPrefab != null)
            {
               GameObject hitVFX = Instantiate(hitVFXPrefab, other.ClosestPoint(transform.position), Quaternion.identity);
               Destroy(hitVFX, 2f);
            }
            damageable.TakeDamage(damage);
            damagedEntities.Add(damageable);
        }
    }
}
