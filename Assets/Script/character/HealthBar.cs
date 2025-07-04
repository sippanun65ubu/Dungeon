using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public Text healthCounter;

    public GameObject playerState;

    public float currentHealth, maxHealth;
    // Start is called before the first frame update
    public void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    public void Update()
    {
        currentHealth = playerState.GetComponent<PlayerState>().currentHealth;
        maxHealth = playerState.GetComponent<PlayerState>().maxHealth;

        float fillvalue = currentHealth / maxHealth;
        slider.value = fillvalue;

        healthCounter.text = (int)currentHealth + "/" + (int)maxHealth;
    }
}
