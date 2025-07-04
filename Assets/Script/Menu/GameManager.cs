using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int totalScore = 0; // Track total score
    public TextMeshProUGUI timerText;
    public bool isPaused = false;

    // Countdown: 30 minutes = 1800 seconds.
    public float remainingTime = 1800f;

    [Header("UI References (End Game)")]
    public GameObject endPage;
    public TextMeshProUGUI TotalScoreText;
    public TextMeshProUGUI ElapsedTimeText;
    public Button SubmitButton;

    [Header("Main Menu Scene")]
    public string mainMenuSceneName = "MainMenu";
    public int requiredScore = 1000;
    private bool hasTriggeredEndGame = false;
    private bool hasActivatedChild = false;
    private bool bonusAwarded = false;

    [Header("Dont need end game panel")]
    public GameObject TimerPanel;
    public GameObject MinimapPanel;
    public GameObject QuickSlotPanel;
    public GameObject StatBarPanel;

    public void Awake()
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

    public void Start()
    {
        endPage.SetActive(false);
        SubmitButton.onClick.AddListener(OnEndSubmitButtonClicked);
    }

    // Update is called once per frame
    public void Update()
    {
        if (!isPaused && remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            remainingTime = Mathf.Max(remainingTime, 0f);

            int mins = Mathf.FloorToInt(remainingTime / 60f);
            int secs = Mathf.FloorToInt(remainingTime % 60f);
            if (timerText != null)
                timerText.text = $"{mins:00}:{secs:00}";
        }
        else if (remainingTime <= 0f && timerText != null)
        {
            timerText.text = "00:00";
            if (!hasTriggeredEndGame)
            {
                hasTriggeredEndGame = true;
                ShowEndGamePage(isDeath: true);
            }
        }

        //Check for player death
        if (!hasTriggeredEndGame && PlayerState.Instance.isPlayerDead)
        {
            hasTriggeredEndGame = true;
            ShowEndGamePage(isDeath: true);
        }
        //If score reaches requirement, activate exactly one child randomly
        if (!hasActivatedChild && totalScore >= requiredScore)
        {
            ActivateRandomChild();
        }
    }

    public void ShowEndGamePage(bool isDeath)
    {
        isPaused = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SelectionManager.Instance.DisableSelection();
        SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;
        MovementManager.instance.EnableLook(false);
        MovementManager.instance.EnableMovement(false);
        TimerPanel.SetActive(false);
        MinimapPanel.SetActive(false);
        QuickSlotPanel.SetActive(false);
        StatBarPanel.SetActive(false);

        endPage.SetActive(true);

        if (!isDeath && !bonusAwarded)
        {
            int minutesLeft = Mathf.FloorToInt(remainingTime / 60f);
            int bonusPoints = minutesLeft * 300;
            totalScore += bonusPoints;
            bonusAwarded = true;
        }
        TotalScoreText.text = "Total Score: " + totalScore;

        float elapsedSeconds = 1800f - remainingTime;
        int hours = Mathf.FloorToInt(elapsedSeconds / 3600f);
        int minutes = Mathf.FloorToInt((elapsedSeconds % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(elapsedSeconds % 60f);

        if (ElapsedTimeText != null)
            ElapsedTimeText.text = "Play Time: " +
                string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
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

    public void OnEndSubmitButtonClicked()
    {
        // Send leaderboard stats to PlayFab
        if (PLayFabManager.Instance != null)
        {
            PLayFabManager.Instance.SendGameStatsToPlayFab();
        }
        else
        {
            Debug.LogError("PLayFabManager instance is null! Cannot send stats.");
        }

        // Load the main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }
    public void ActivateRandomChild()
    {
        int childCount = transform.childCount;
        if (childCount == 0) return;

        int idx = UnityEngine.Random.Range(0, childCount);
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            bool shouldActivate = (i == idx);
            child.gameObject.SetActive(shouldActivate);
        }

        hasActivatedChild = true;
        Debug.Log($"Activated child #{idx} ({transform.GetChild(idx).name}) because score = {totalScore}");
    }
    public void ChildTriggerEntered(Collider other)
    {
        if (!hasActivatedChild) return;
        if (!other.CompareTag("Player")) return;
        if (PlayerState.Instance != null && PlayerState.Instance.isPlayerDead == false)
        {
            if (!hasTriggeredEndGame)
            {
                hasTriggeredEndGame = true;
                ShowEndGamePage(isDeath: false);
            }
        }
    }

    public void ResetToDefaults()
    {
        isPaused = false;
        totalScore = 0;
        remainingTime = 1800f;

    }
}
