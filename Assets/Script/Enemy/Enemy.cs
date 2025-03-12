using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public bool playerInRange;

    private Animator animator;
    public bool isDead = false;
    [SerializeField] float currentHealth;
    [SerializeField] float maxHealth;

    private NavMeshAgent agent;

    public Slider healthSlider;

    public bool isSuperregen = false; // Enable/disable super regeneration
    public float healthRegenRate = 5f; // Health regenerated per second
    public float healthRegenDelay = 3f; 
    private float lastDamageTime; // Track when the enemy last took damage

    public int enemyScoreValue = 10;


    enum EnemyType
    {
        Skeleton,
        Bat,
        Slime
    }
    EnemyType thisenemyType;

    [Header("Sound")]
    [SerializeField] AudioSource soundChannel;
    [SerializeField] AudioClip skeletonTakeDamage;
    [SerializeField] AudioClip skeletonDie;
    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();  
        agent = GetComponent<NavMeshAgent>();

    }
    private void Update()
    {
        healthSlider.value = currentHealth / maxHealth;

        if (isSuperregen && !isDead)
        {
            HandleHealthRegeneration();
        }

    }


    private void HandleHealthRegeneration()
    {
        // Check if enough time has passed since the last damage
        if (Time.time - lastDamageTime > healthRegenDelay && currentHealth < maxHealth)
        {
            currentHealth += healthRegenRate * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            healthSlider.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            healthSlider.gameObject.SetActive(false);
        }
    }
    public void TakeDamage(int damage)
    {
        if (isDead == false)
        {
            currentHealth -= damage;
            healthSlider.value = currentHealth / maxHealth;
            if (currentHealth <= 0)
            {
                PlayDyingSound();
                animator.SetTrigger("DIE");
                agent.enabled = false;

                isDead = true;
                healthSlider.gameObject.SetActive(false);
                GameManager.instance.AddKill(enemyScoreValue);
            }
            else
            {
                PlayHitSound();
                animator.SetTrigger("HURT");
            }
        }

    }


    private void PlayHitSound()
    {
        switch(thisenemyType)
        {
            case EnemyType.Skeleton:
                soundChannel.PlayOneShot(skeletonTakeDamage);
                break;
            //case :
            //    soundChannel.PlayOneShot();
            //    break;
        }
    }

    private void PlayDyingSound()
    {
        switch(thisenemyType)
        {
            case EnemyType.Skeleton:
                soundChannel.PlayOneShot(skeletonDie);
                break;
            //case :
            //    soundChannel.PlayOneShot();
            //    break;
        }
    }


}
