using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    private Slider slider;
    public Text staminaCounter;

    public GameObject playerState;

    public float currentStamina, maxStamina;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        currentStamina = playerState.GetComponent<PlayerState>().currentStamina;
        maxStamina = playerState.GetComponent<PlayerState>().maxStamina;

        float fillvalue = currentStamina / maxStamina;
        slider.value = fillvalue;

        staminaCounter.text = currentStamina + "/" + maxStamina;
    }
}
