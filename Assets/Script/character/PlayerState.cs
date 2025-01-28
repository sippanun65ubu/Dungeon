using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float healthRegenRate = 2f; // Health regenerated per second
    public float healthRegenDelay = 5f; // Delay before health regeneration starts

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 20f; // Stamina drained per second while sprinting
    public float staminaRegenRate = 10f; // Stamina regenerated per second
    public float staminaRegenDelay = 2f; // Delay before stamina regeneration starts

    [Header("UI Elements")]
    public Slider healthBar;
    public Text healthText; // New health text component
    public Slider staminaBar;
    public Text staminaText; // New stamina text component

    private float currentHealth;
    private float currentStamina;
    private float lastDamageTime;
    private bool isSprinting;

    private void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (staminaBar != null)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = currentStamina;
        }

        UpdateUI();
    }

    private void Update()
    {
        HandleHealthRegeneration();
        HandleStamina();
        UpdateUI();

        //controls for testing
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Heal(10);
        }

        isSprinting = Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0;
    }

    private void HandleHealthRegeneration()
    {
        if (Time.time - lastDamageTime > healthRegenDelay && currentHealth < maxHealth)
        {
            currentHealth += healthRegenRate * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
    }

    private void HandleStamina()
    {
        if (isSprinting && currentStamina > 0)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0);
        }
        else if (!isSprinting && currentStamina < maxStamina)
        {
            if (Time.time - lastDamageTime > staminaRegenDelay)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);
            }
        }
    }

    private void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{Mathf.Ceil(currentHealth)} / {maxHealth}";
        }

        if (staminaBar != null)
        {
            staminaBar.value = currentStamina;
        }

        if (staminaText != null)
        {
            staminaText.text = $"{Mathf.Ceil(currentStamina)} / {maxStamina}";
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        lastDamageTime = Time.time;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player Died");
        // Add death handling logic
    }
}
