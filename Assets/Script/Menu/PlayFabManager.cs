using System.Collections;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class PLayFabManager : MonoBehaviour
{
    #region Awake
    public static PLayFabManager Instance { get; private set; }
    private const string PlayerDataKey = "PlayerData";

    private void Awake()
    {
        // Setup singleton.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region  Field
    //      UI & Login Section   //
    [Header("Login UI")]
    [SerializeField] InputField loginEmail;
    [SerializeField] InputField loginPassword;

    [Header("Signup UI")]
    [SerializeField] InputField signupUsername;
    [SerializeField] InputField signupEmail;
    [SerializeField] InputField signupPassword;
    [SerializeField] InputField signupCPassword;

    [Header("Forget Password UI")]
    [SerializeField] InputField forgetPasswordEmail;
    [SerializeField] Text messageText;
    #endregion


    #region Registration
    public void RegisterUser()
    {
        // Optionally check that signupPassword and signupCPassword match and have minimum length.
        var request = new RegisterPlayFabUserRequest
        {
            DisplayName = signupUsername.text,
            Email = signupEmail.text,
            Password = signupPassword.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        messageText.text = "New Account Created";
        FirstMenuManager.Instance.LoginScreen();
    }
    #endregion

    #region Login
    public void Login()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = loginEmail.text,
            Password = loginPassword.text,
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        messageText.text = "Logged In";
        FirstMenuManager.Instance.MainMenuScreen();
    }
    #endregion

    #region Recovery
    public void RecoverUser()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = forgetPasswordEmail.text,
            TitleId = "YOUR_TITLE_ID" // Replace with your actual TitleId.
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnRecoverySuccess, OnErrorRecovery);
    }

    private void OnErrorRecovery(PlayFabError error)
    {
        messageText.text = "No Email Found";
    }

    private void OnRecoverySuccess(SendAccountRecoveryEmailResult result)
    {
        messageText.text = "Recovery Email Sent";
        FirstMenuManager.Instance.LoginScreen();
    }

    private void OnError(PlayFabError error)
    {
        messageText.text = error.ErrorMessage;
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion

    #region  plater data
    [System.Serializable]
    public class PlayerData
    {
        public float[] playerStats;              // [0]: health, [1]: stamina
        public float[] playerPositionAndRotation; // [0-2]: position, [3-5]: forward direction (for LookRotation)
        public string[] inventoryContent;
        public string[] quickSlotContent;
        public int enemyKillCount;
        public int totalScore;
        public float elapsedTime;
        public string currentScene;


        public PlayerData(float[] _playerStats, float[] _playerPosAndRot, string[] _inventoryContent, string[] _quickSlotContent, int _enemyKillCount, int _totalScore, float _elapsedTime, string _currentScene)
        {
            playerStats = _playerStats;
            playerPositionAndRotation = _playerPosAndRot;
            inventoryContent = _inventoryContent;
            quickSlotContent = _quickSlotContent;
            enemyKillCount = _enemyKillCount;
            totalScore = _totalScore;
            elapsedTime = _elapsedTime;
            currentScene = _currentScene;
        }
    }
    #endregion

    #region SaveDataPlayFab
    public void SavePlayerData()
    {
        PlayerData data = CreatePlayerData();
        string jsonData = JsonUtility.ToJson(data);
        Debug.Log("Saving Player Data: " + jsonData);

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>()
            {
                { PlayerDataKey, jsonData }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnDataSaved, OnDataError);
    }

    private void OnDataSaved(UpdateUserDataResult result)
    {
        Debug.Log("Player data saved successfully.");
    }

    private void OnDataError(PlayFabError error)
    {
        Debug.LogError("Error saving/loading player data: " + error.GenerateErrorReport());
    }
    #endregion

    #region LoadData
    public void LoadPlayerData(Action<PlayerData> OnDataLoaded)
    {
        var request = new GetUserDataRequest
        {
            Keys = new List<string>() { PlayerDataKey }
        };

        PlayFabClientAPI.GetUserData(request, result =>
        {
            if (result.Data != null && result.Data.ContainsKey(PlayerDataKey))
            {
                string jsonData = result.Data[PlayerDataKey].Value;
                Debug.Log("Loaded Player Data: " + jsonData);
                PlayerData loadedData = JsonUtility.FromJson<PlayerData>(jsonData);
                OnDataLoaded?.Invoke(loadedData);
            }
            else
            {
                Debug.Log("No player data found.");
                OnDataLoaded?.Invoke(null);
            }
        }, OnDataError);
    }

    /// Creates a PlayerData object from the current game state.
    private PlayerData CreatePlayerData()
    {
        // Get player stats from PlayerState.
        float[] playerStats = new float[2];
        playerStats[0] = PlayerState.Instance.currentHealth;
        playerStats[1] = PlayerState.Instance.currentLife;

        // Get player position and rotation.
        float[] posAndRot = new float[6];
        Vector3 pos = PlayerState.Instance.playerBody.transform.position;
        posAndRot[0] = pos.x;
        posAndRot[1] = pos.y;
        posAndRot[2] = pos.z;
        // Store forward direction for rotation.
        Vector3 forward = PlayerState.Instance.playerBody.transform.forward;
        posAndRot[3] = forward.x;
        posAndRot[4] = forward.y;
        posAndRot[5] = forward.z;

        // Get inventory and quick slot content.
        string[] inventory = InventorySystem.Instance.itemList.ToArray();
        string[] quickSlots = GetQuickSlotContents();

        // Get enemy kill count and total score.
        int enemyKills = GameManager.instance.enemyKillCount;
        int totalScore = GameManager.instance.totalScore;

        // Get elapsed time from GameManager.
        float elapsedTime = GameManager.instance.GetElapsedTime();
        // Get current scene name.
        string currentScene = SceneManager.GetActiveScene().name;

        return new PlayerData(playerStats, posAndRot, inventory, quickSlots, enemyKills, totalScore, elapsedTime, currentScene);
    }

    /// Retrieves quick slot contents from EquipSystem.
    private string[] GetQuickSlotContents()
    {
        List<string> temp = new List<string>();

        foreach (GameObject slot in EquipSystem.Instance.quickSlotsList)
        {
            if (slot.transform.childCount != 0)
            {
                string name = slot.transform.GetChild(0).name;
                string cleanName = name.Replace("(Clone)", "").Trim();
                temp.Add(cleanName);
            }
        }
        return temp.ToArray();
    }

    /// Applies loaded player data to the game.
    public IEnumerator SetPlayerDataCoroutine(PlayerData playerData)
    {
        // Check if the saved scene is different from the current scene.
        if (SceneManager.GetActiveScene().name != playerData.currentScene)
        {
            Debug.Log("Loading saved scene: " + playerData.currentScene);
            // Load the scene asynchronously.
            AsyncOperation op = SceneManager.LoadSceneAsync(playerData.currentScene);
            while (!op.isDone)
            {
                yield return null;
            }
            
            yield return null;
        }

        
        PlayerState.Instance.currentHealth = playerData.playerStats[0];
        PlayerState.Instance.currentLife = playerData.playerStats[1];

        Vector3 loadPos = new Vector3(
            playerData.playerPositionAndRotation[0],
            playerData.playerPositionAndRotation[1],
            playerData.playerPositionAndRotation[2]
        );
        Vector3 forward = new Vector3(
            playerData.playerPositionAndRotation[3],
            playerData.playerPositionAndRotation[4],
            playerData.playerPositionAndRotation[5]
        );
        PlayerState.Instance.playerBody.transform.position = loadPos;
        PlayerState.Instance.playerBody.transform.rotation = Quaternion.LookRotation(forward);

        // Restore inventory.
        foreach (string item in playerData.inventoryContent)
        {
            InventorySystem.Instance.AddToInventory(item);
        }

        // Restore quick slot content.
        foreach (string item in playerData.quickSlotContent)
        {
            GameObject availableSlot = EquipSystem.Instance.FindNextEmptySlot();
            GameObject itemToAdd = Instantiate(Resources.Load<GameObject>(item));
            itemToAdd.transform.SetParent(availableSlot.transform, false);
        }

        // Update enemy kill count and total score.
        GameManager.instance.enemyKillCount = playerData.enemyKillCount;
        GameManager.instance.totalScore = playerData.totalScore;

        // Update elapsed time.
        GameManager.instance.SetElapsedTime(playerData.elapsedTime);

        Debug.Log("Player data applied.");
    }
    #endregion LeaderBoard

    #region LeaderBoard 

    public void SendGameStatsToPlayFab()
    {
        // Retrieve values from your GameManager (assumes singleton instance).
        int totalScore = GameManager.instance.totalScore;
        int enemyKills = GameManager.instance.enemyKillCount;
        // Convert elapsedTime to an integer value (seconds).
        int elapsedTimeSeconds = Mathf.FloorToInt(GameManager.instance.GetElapsedTime());

        // Build the request with multiple statistics.
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = "DungeonScore", Value = totalScore },
                new StatisticUpdate { StatisticName = "KillCount", Value = enemyKills },
                new StatisticUpdate { StatisticName = "Time", Value = elapsedTimeSeconds }
            }
        };

        // Send the request to PlayFab.
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnStatsUpdated, OnStatsError);
    }

    private void OnStatsUpdated(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Game statistics updated successfully.");
    }

    private void OnStatsError(PlayFabError error)
    {
        Debug.LogError("Error updating game statistics: " + error.GenerateErrorReport());
    }

    #endregion
}