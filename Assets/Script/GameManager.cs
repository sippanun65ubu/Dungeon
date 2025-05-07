using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int totalScore = 0; // Track total score
    [SerializeField] TextMeshProUGUI timerText;
    public bool isPaused = false;

    // Countdown: 30 minutes = 1800 seconds.
    public float remainingTime = 1800f;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddKill(int scoreValue)
    {
        totalScore += scoreValue;
    }

    public void AddQuestScore(int scoreValue)
    {
        totalScore += scoreValue;
    }


    // Update is called once per frame
    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            remainingTime = Mathf.Max(remainingTime, 0f);  // Avoid negative time

            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            // Timer has reached zero—optional: trigger game over.
            timerText.text = "00:00";
            // Example: GameOver();
        }
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
    public float GetRemainingTime()
    {
        return remainingTime;
    }

    public void SetRemainingTime(float time)
    {
        remainingTime = time;
    }

}
