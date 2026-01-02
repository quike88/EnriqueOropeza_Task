using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Ajustes Visuales")]
    [SerializeField] private float lerpSpeed = 5f;

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
}