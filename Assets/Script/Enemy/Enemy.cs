using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public bool playerInRange;

    public Animator animator;
    public bool isDead = false;
    [SerializeField] private float _currentHealth;
    public float currentHealth
    {
        get => _currentHealth;
        set => _currentHealth = value;
    }
    public float maxHealth;

    public NavMeshAgent agent;

    public Slider healthSlider;

    public bool isSuperregen = false; 
    public float healthRegenRate = 5f; 
    public float healthRegenDelay = 3f;
    public float lastDamageTime; 

    public float damageToInflict; 
    public float currentDamageToInflict;
    public int armor = 0;
    public int maxarmor = 5;

    public string enemyId;

    public string resourcePath;
    public enum EnemyType
    {
        Skeleton,
        Bat,
        Slime
    }
    public EnemyType thisenemyType;

    [Header("Sound")]
    [SerializeField] AudioSource soundChannel;
    [SerializeField] AudioClip monsterTakeDamage;
    [SerializeField] AudioClip monsterDie;


    public int enemyScoreValue = 10;

    public void Awake()
    {
        if (string.IsNullOrEmpty(enemyId))
        {
            // Generate a new GUID and store it
            enemyId = Guid.NewGuid().ToString();
        }
    }

    public void Start()
    {
        currentHealth = maxHealth;
        currentDamageToInflict = damageToInflict;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

    }
    public void Update()
    {
        healthSlider.value = currentHealth / maxHealth;

        if (isSuperregen && !isDead)
        {
            HandleHealthRegeneration();
        }

    }


    public void HandleHealthRegeneration()
    {
        // Check if enough time has passed since the last damage
        if (Time.time - lastDamageTime > healthRegenDelay && currentHealth < maxHealth)
        {
            currentHealth += healthRegenRate * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            healthSlider.gameObject.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            healthSlider.gameObject.SetActive(false);
        }
    }
    public void TakeDamage(int damage, int penetration)
    {

        if (isDead == false)
        {
            int effectiveArmor = Mathf.Clamp(armor - penetration, 0, maxarmor);
            float reductionPercentage = (float)effectiveArmor / (maxarmor * 2f);
            int effectiveDamage = Mathf.CeilToInt(damage * (1 - reductionPercentage));
            currentHealth -= effectiveDamage;
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


    public void PlayHitSound()
    {
        switch (thisenemyType)
        {
            case EnemyType.Skeleton:
                soundChannel.PlayOneShot(monsterTakeDamage);
                break;
                //case :
                //    soundChannel.PlayOneShot();
                //    break;
        }
    }

    public void PlayDyingSound()
    {
        switch (thisenemyType)
        {
            case EnemyType.Skeleton:
                soundChannel.PlayOneShot(monsterDie);
                break;
                //case :
                //    soundChannel.PlayOneShot();
                //    break;
        }
    }
    public void Attack()
    {
        PlayerState.Instance.TakeDamage(currentDamageToInflict);
    }


    public void ForceDieImmediate()
    {
        isDead = true;
        agent.enabled = false;
        animator.SetTrigger("DIE");
        healthSlider.gameObject.SetActive(false);
    }
}
