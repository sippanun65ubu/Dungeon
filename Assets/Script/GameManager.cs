using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int enemyKillCount = 0; // Track enemy kills
    public int totalScore = 0; // Track total score

    //public Text killCounterText; // Assign in Inspector
    //public Text scoreText; // Assign in Inspector

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddKill(int scoreValue)
    {
        enemyKillCount++;
        totalScore += scoreValue;
        //UpdateUI();
    }

    public void AddQuestScore(int scoreValue)
    {
        totalScore += scoreValue;
        //UpdateUI();
    }

    //void UpdateUI()
    //{
    //    if (killCounterText)
    //        killCounterText.text = "Kills: " + enemyKillCount;

    //    if (scoreText)
    //        scoreText.text = "Score: " + totalScore;
    //}
}
