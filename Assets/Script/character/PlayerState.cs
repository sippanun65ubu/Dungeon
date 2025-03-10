using System;
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
    public bool isPlayerDead;

    public AudioSource playerAudioSource;
    public AudioClip playerPainSound;
    public AudioClip playerDeathSound;

    public RespawnLocation spawnLocation;
    public GameObject playerBody;

    private float hurtSoundDelay = 2f;
    private float nextHurtTime = 0f;

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
    public void setHealth(float newHealth)
    {
        currentHealth = newHealth;
    }
    public void setStamina(float newStamina)
    {
        currentStamina = newStamina;
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0 && !isPlayerDead)
        {
            Debug.Log("player is dead");
            PlayerDead();
        }
        else
        {
            if (currentHealth > 0 && Time.time >= nextHurtTime)
            {
                Debug.Log("player is hurt");
                playerAudioSource.PlayOneShot(playerPainSound);

                nextHurtTime = Time.time + hurtSoundDelay;
            }



        }
    }
    public void PlayerDead()
    {
        isPlayerDead = true;
        playerAudioSource.PlayOneShot(playerDeathSound);
        RespawnPlayer();
    }
    public void RespawnPlayer()
    {
        StartCoroutine(RespawnCoroutine());
    }
    public IEnumerator RespawnCoroutine()
    {
        playerBody.GetComponent<PlayerMovement>().enabled = false;
        //playerBody.GetComponent<MouseMovement>().enabled = false;

        Vector3 position = spawnLocation.transform.position;

        position.y += 5f;

        playerBody.transform.position = position;

        currentHealth = maxHealth;


        yield return new WaitForSeconds(0.2f);

        isPlayerDead = false;

        //playerBody.GetComponent<PlayerMovement>().enabled = true;
        //playerBody.GetComponent<MouseMovement>().enabled = true;
    }

    internal void SpawnPlayerLocation(RespawnLocation respawnLocation)
    {
         spawnLocation = respawnLocation;
            
    }
}
