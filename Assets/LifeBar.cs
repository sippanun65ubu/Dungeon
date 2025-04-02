using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{

    private Slider slider;
    public Text lifeCounter;

    public GameObject playerState;

    public float currentLife, maxLife;
    // Start is called before the first frame update
    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        currentLife = playerState.GetComponent<PlayerState>().currentLife;
        maxLife = playerState.GetComponent<PlayerState>().maxLife;

        float fillvalue = currentLife / maxLife;
        slider.value = fillvalue;
        lifeCounter.text = (int)currentLife + "/" + (int)maxLife;
    }
}
