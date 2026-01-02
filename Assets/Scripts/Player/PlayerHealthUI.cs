using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private float lerpSpeed = 5f;

    [Header("Damage effect")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashIntensity = 0.5f;
    [SerializeField] private float flashDuration = 0.2f;

    private Vignette vignette;
    private float lastHealth;
    private Color originalVignetteColor;
    private float originalVignetteIntensity;

    private void Start()
    {
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerHealth = player.GetComponent<Health>();
        }

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthBar;

            healthSlider.maxValue = 1f;
            healthSlider.value = 1f;
            lastHealth = playerHealth.currentHealthValue();
        }
        if (globalVolume != null && globalVolume.profile.TryGet<Vignette>(out vignette))
        {
            originalVignetteColor = vignette.color.value;
            originalVignetteIntensity = vignette.intensity.value;
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        float targetFill = currentHealth / maxHealth;
        StopAllCoroutines();
        StartCoroutine(AnimateBar(targetFill));
        if (currentHealth < lastHealth)
        {
            StopCoroutine("FlashVignette");
            StartCoroutine(FlashVignette());
        }

        lastHealth = currentHealth;
    }

    private IEnumerator AnimateBar(float targetFill)
    {
        while (Mathf.Abs(healthSlider.value - targetFill) > 0.001f)
        {
            healthText.text = $"{Mathf.RoundToInt(healthSlider.value * playerHealth.maxHealthValue())} / {Mathf.RoundToInt(playerHealth.maxHealthValue())}";
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetFill, Time.deltaTime * lerpSpeed);
            yield return null;
        }
        healthSlider.value = targetFill;
    }
    private IEnumerator FlashVignette()
    {
        if (vignette == null) yield break;

        // Configurar color de daño e intensidad máxima
        vignette.color.Override(damageColor);

        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flashDuration;

            // Subida y bajada rápida de intensidad
            float currentIntensity = Mathf.Lerp(originalVignetteIntensity, flashIntensity, Mathf.Sin(t * Mathf.PI));
            vignette.intensity.Override(currentIntensity);

            yield return null;
        }

        // Restaurar valores originales
        vignette.color.Override(originalVignetteColor);
        vignette.intensity.Override(originalVignetteIntensity);
    }
}