using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; set; }

    [Header("Health Settings")]
    public float maxHealth;
    public float currentHealth;
    public float healthRegenRate; // Health regenerated per second
    public float healthRegenDelay; // Delay before health regeneration starts

    private float lastDamageTime;
    public bool isSprinting;
    public bool isPlayerDead;

    [Header("Life")]
    public float maxLife;
    public float currentLife;
    public float baseLifeDrainRate = 1f;
    public float lifeDrainTimeThreshold = 300f;
    public float increasedLifeDrainRate = 2f;


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
        currentLife = maxLife;

    }

    private void Update()
    {
        HandleHealthRegeneration();
        HandleLifeDrain();
    }

    private void HandleHealthRegeneration()
    {
        if (Time.time - lastDamageTime > healthRegenDelay && currentHealth < maxHealth)
        {
            currentHealth += healthRegenRate * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
    }

    private void HandleLifeDrain()
    {
        // Ensure GameManager exists.
        float elapsedTime = GameManager.instance != null ? GameManager.instance.elapsedTime : 0f;
        // Choose drain rate based on elapsed time.
        float currentDrainRate = (elapsedTime >= lifeDrainTimeThreshold) ? increasedLifeDrainRate : baseLifeDrainRate;

        currentLife -= currentDrainRate * Time.deltaTime;
        currentLife = Mathf.Max(currentLife, 0); // Prevent negative life

        //If life reaches zero, trigger endgame.
        if (currentLife <= 0 && !isPlayerDead)
        {
            Debug.Log("Player's life has drained completely.");
            SceneManager.LoadScene("EndGame");
        }
    }
    public void setHealth(float newHealth)
    {
        currentHealth = newHealth;
    }

    public void setLife(float newLife)
    {
        currentLife = newLife;
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
        currentLife = Mathf.Max(currentLife - 100, 0);
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