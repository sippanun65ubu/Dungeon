using System;
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

    public RespawnLocation spawnLocation;
    public GameObject playerBody;

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

    }

    private void Update()
    {
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

    public void setHealth(float newHealth)
    {
        currentHealth = newHealth;
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0 && !isPlayerDead)
        {
            Debug.Log("player is dead");
            PlayerDead();
        }
        else
        {
                Debug.Log("player is hurt");
                playerAudio.PlayOneShot(playerHurt);
        }
    }

    public void PlayerDead()
    {
        isPlayerDead = true;
        playerAudio.PlayOneShot(playerDie);
    }

    internal void ResetToDefaults()
    {
        var pm = playerBody.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;
        var cc = playerBody.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        currentHealth = maxHealth;
        isPlayerDead = false;
        Vector3 spawnPos = spawnLocation.transform.position;
        spawnPos.y += 3f;
        playerBody.transform.position = spawnPos;
        if (pm != null) pm.enabled = true;
        if (cc != null) cc.enabled = true;
    }

    //public void RespawnPlayer()
    //{
    //    StartCoroutine(RespawnCoroutine());
    //}

    //public IEnumerator RespawnCoroutine()
    //{
    //    playerBody.GetComponent<PlayerMovement>().enabled = false;

    //    Vector3 position = spawnLocation.transform.position;

    //    position.y += 3f;

    //    playerBody.transform.position = position;

    //    currentHealth = maxHealth;

    //    yield return new WaitForSeconds(0.2f);

    //    isPlayerDead = false;

    //    playerBody.GetComponent<PlayerMovement>().enabled = true;
    //}

    internal void SpawnPlayerLocation(RespawnLocation respawnLocation)
    {
        spawnLocation = respawnLocation;
    }

}