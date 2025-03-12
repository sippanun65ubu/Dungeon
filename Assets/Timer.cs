using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    float elapsedTime;

    // Update is called once per frame
    void Update()
    {
        int hours = Mathf.FloorToInt(elapsedTime / 3600); // 1 hour = 3600 seconds
        int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60); // 1 minute = 60 seconds
        int seconds = Mathf.FloorToInt(elapsedTime % 60); // Remainder is seconds


        elapsedTime += Time.deltaTime;
        timerText.text = elapsedTime.ToString();
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
    public void Pause()
    {
        //pauseMenu.setActive(true);
        Time.timeScale = 0;
    }
    

}
