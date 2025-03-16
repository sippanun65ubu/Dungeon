using System.Collections;
using UnityEngine;

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
    public float staminaDrainRate = 1f; // Stamina drained per second while sprinting (slower drain)
    public float staminaRegenRate = 2f; // Stamina regenerated per second
    public float staminaRegenDelay = 1f; // Delay before stamina regeneration starts after stopping sprinting

    private float lastDamageTime;
    private float lastSprintEndTime; // Track when the player stopped sprinting
    public bool isSprinting;
    public bool isPlayerDead;

    public RespawnLocation spawnLocation;
    public GameObject playerBody;

    private float hurtSoundDelay = 2f;
    private float nextHurtTime = 0f;

    public AudioSource playerAudio;
    public AudioClip playerHurt;
    public AudioClip playerDie;
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
        // Handle stamina drain while sprinting
        if (isSprinting && currentStamina > 0)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0);
        }

        // Handle stamina regeneration
        if (!isSprinting && currentStamina < maxStamina)
        {
            // Check if enough time has passed since the player stopped sprinting
            if (Time.time - lastSprintEndTime > staminaRegenDelay)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);
            }
        }

        // Update lastSprintEndTime when the player stops sprinting
        if (!isSprinting && lastSprintEndTime == 0)
        {
            lastSprintEndTime = Time.time;
        }
        else if (isSprinting)
        {
            lastSprintEndTime = 0; // Reset the timer if the player starts sprinting again
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
                playerAudio.PlayOneShot(playerHurt);

                nextHurtTime = Time.time + hurtSoundDelay;
            }
        }
    }

    public void PlayerDead()
    {
        isPlayerDead = true;
        playerAudio.PlayOneShot(playerDie);
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

        position.y += 3f;

        playerBody.transform.position = position;

        currentHealth = maxHealth;

        yield return new WaitForSeconds(0.2f);

        isPlayerDead = false;

        playerBody.GetComponent<PlayerMovement>().enabled = true;
        //playerBody.GetComponent<MouseMovement>().enabled = true;
    }

    internal void SpawnPlayerLocation(RespawnLocation respawnLocation)
    {
        spawnLocation = respawnLocation;
    }
}