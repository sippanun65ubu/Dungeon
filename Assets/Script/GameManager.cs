using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int enemyKillCount = 0; // Track enemy kills
    public int totalScore = 0; // Track total score
    [SerializeField] TextMeshProUGUI timerText;
    public float elapsedTime;
    public bool isPaused = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddKill(int scoreValue)
    {
        enemyKillCount++;
        totalScore += scoreValue;
    }

    public void AddQuestScore(int scoreValue)
    {
        totalScore += scoreValue;
    }


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
        isPaused = true;
        Time.timeScale = 0;
    }
    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }
    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void SetElapsedTime(float time)
    {
        elapsedTime = time;
    }

}
