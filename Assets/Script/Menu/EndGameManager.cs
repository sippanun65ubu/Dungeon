using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI killCountText;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI elapsedTimeText;
    [SerializeField] private Button submitButton;

    [Header("Main Menu Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start()
    {

        if (GameManager.instance != null)
        {

            // Score
            totalScoreText.text = "Total Score: " + GameManager.instance.totalScore;

            // Time
            float time = GameManager.instance.GetRemainingTime();
            int hours = Mathf.FloorToInt(time / 3600);
            int minutes = Mathf.FloorToInt((time % 3600) / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            elapsedTimeText.text = "Play Time: " + string.Format("Time: {0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitButtonClicked);
        }
    }

    private void OnSubmitButtonClicked()
    {
        // Send leaderboard stats to PlayFab.
        if (PLayFabManager.Instance != null)
        {
            PLayFabManager.Instance.SendGameStatsToPlayFab();
        }
        else
        {
            Debug.LogError("PLayFabManager instance is null! Cannot send stats.");
        }

        // Load the main menu scene.
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
