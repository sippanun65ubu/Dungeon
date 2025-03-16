using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [Header("Leaderboard UI References")]
    [Tooltip("The parent container (e.g., Content under a ScrollRect) where leaderboard entries will be placed.")]
    [SerializeField] private Transform leaderboardContent;

    [Tooltip("Prefab for each leaderboard entry (with RankText, NameText, ScoreText).")]
    [SerializeField] private GameObject leaderboardRowPrefab;

    [Tooltip("Maximum number of leaderboard entries to retrieve from PlayFab.")]
    [SerializeField] private int maxResultsCount = 10;

    public void LoadKillCountLeaderboard()
    {
        LoadLeaderboard("KillCount");
    }

    public void LoadDungeonScoreLeaderboard()
    {
        LoadLeaderboard("DungeonScore");
    }
    public void LoadTimeLeaderboard()
    {
        LoadLeaderboard("Time");
    }

    private void LoadLeaderboard(string statisticName)
    {
        Debug.Log($"Requesting leaderboard for: {statisticName}");

        var request = new GetLeaderboardRequest
        {
            StatisticName = statisticName,
            StartPosition = 0,
            MaxResultsCount = maxResultsCount
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnGetLeaderboardError);
    }


    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        Debug.Log("Leaderboard data retrieved successfully.");

        // Clear any existing rows in the UI.
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // For each entry in the leaderboard, create a row and populate it.
        foreach (var entry in result.Leaderboard)
        {
            // Instantiate a new row prefab under the leaderboardContent parent.
            GameObject rowGO = Instantiate(leaderboardRowPrefab, leaderboardContent);

            // Find the UI elements (TextMeshProUGUI) in the row prefab.
            TextMeshProUGUI rankText = rowGO.transform.Find("RankText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI nameText = rowGO.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI scoreText = rowGO.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();


            // The rank is zero-based, so add 1 for display.
            rankText.text = (entry.Position + 1).ToString();

            nameText.text = string.IsNullOrEmpty(entry.DisplayName) ? entry.PlayFabId : entry.DisplayName;

            scoreText.text = entry.StatValue.ToString();

            Debug.Log($"Rank: {entry.Position + 1}, Name: {nameText.text}, Value: {entry.StatValue}");
        }
    }

    private void OnGetLeaderboardError(PlayFabError error)
    {
        Debug.LogError("Error retrieving leaderboard: " + error.GenerateErrorReport());
    }
}
