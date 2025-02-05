using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; set; }

    [Header("Health Settings")]
    public float maxHealth;
    public float currentHealth; 
    public float healthRegenRate; // Health regenerated per second
    public float healthRegenDelay; // Delay before health regeneration starts

    [Header("Stamina Settings")]
    public float maxStamina;
    public float currentStamina;
    public float staminaDrainRate; // Stamina drained per second while sprinting
    public float staminaRegenRate; // Stamina regenerated per second
    public float staminaRegenDelay; // Delay before stamina regeneration starts

    //[Header("UI Elements")]
    //public Slider healthBar;
    //public Text healthText; // New health text component
    //public Slider staminaBar;
    //public Text staminaText; // New stamina text component

    private float lastDamageTime;
    public bool isSprinting;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

    }

    private void Update()
    {
        HandleStamina();
        HandleHealthRegeneration();
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
}
