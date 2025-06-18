using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PLayFabManager : MonoBehaviour
{
    #region Awake
    public static PLayFabManager Instance { get; set; }


    private void Awake()
    {

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


    #region Registration
    public void RegisterUser()
    {
        // Optionally check that signupPassword and signupCPassword match and have minimum length.
        var request = new RegisterPlayFabUserRequest
        {
            DisplayName = FirstMenuManager.Instance.signupUsername.text,
            Email = FirstMenuManager.Instance.signupEmail.text,
            Password = FirstMenuManager.Instance.signupPassword.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        FirstMenuManager.Instance.messageText.text = "New Account Created";
        FirstMenuManager.Instance.LoginScreen();
    }
    #endregion

    #region Login
    public void Login()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = FirstMenuManager.Instance.loginEmail.text,
            Password = FirstMenuManager.Instance.loginPassword.text,
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        FirstMenuManager.Instance.messageText.text = "Logged In";
        FirstMenuManager.Instance.MainMenuScreen();
    }
    #endregion

    #region Recovery
    public void RecoverUser()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = FirstMenuManager.Instance.forgetPasswordEmail.text,
            TitleId = "YOUR_TITLE_ID" // Replace with your actual TitleId.
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnRecoverySuccess, OnErrorRecovery);
    }

    private void OnErrorRecovery(PlayFabError error)
    {
        FirstMenuManager.Instance.messageText.text = "No Email Found";
    }

    private void OnRecoverySuccess(SendAccountRecoveryEmailResult result)
    {
        FirstMenuManager.Instance.messageText.text = "Recovery Email Sent";
        FirstMenuManager.Instance.LoginScreen();
    }

    private void OnError(PlayFabError error)
    {
        FirstMenuManager.Instance.messageText.text = error.ErrorMessage;
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion

    #region  player data
    [Serializable]
    public class FullSaveData
    {
        public PlayerData player;
        public EnemyData[] enemies;  
        public LootData[] loot;
    }
    [Serializable]
    public class PlayerData
    {
        public float[] playerStats;              // [0]: health
        public float[] playerPositionAndRotation; // [0-2]: position, [3-5]: forward direction (for LookRotation)
        public string[] inventoryContent;
        public string[] quickSlotContent;
        public int totalScore;
        public float remainingTime;
        public string currentScene;


        public PlayerData(float[] _playerStats, float[] _playerPosAndRot, string[] _inventoryContent, string[] _quickSlotContent, int _totalScore, float _remainingTime, string _currentScene)
        {
            playerStats = _playerStats;
            playerPositionAndRotation = _playerPosAndRot;
            inventoryContent = _inventoryContent;
            quickSlotContent = _quickSlotContent;
            totalScore = _totalScore;
            remainingTime = _remainingTime;
            currentScene = _currentScene;
        }

        public static PlayerData CreateDefaultData()
        {

            Vector3 spawnposition = PlayerState.Instance.spawnLocation.transform.position;

            float[] defaultStats = new float[1] { 300f };              
            float[] defaultPosAndRot = new float[6] { spawnposition.x, spawnposition.y, spawnposition.z, 0f, 0f, 1f }; 
            string[] defaultInventory = new string[0];                       
            string[] defaultQuickSlots = new string[0];                        
            int defaultTotalScore = 0;
            float defaultRemainingTime = 1800f;                                
            string defaultScene = "TownNo2";                             

            return new PlayerData(defaultStats, defaultPosAndRot, defaultInventory, defaultQuickSlots, defaultTotalScore, defaultRemainingTime, defaultScene);
        }
    }
    [Serializable]
    public class EnemyData
    {
        public string enemyId;     
        public string prefabName;     
        public float[] position;      
        public float currentHealth;
        public bool isDead;

        public EnemyData(string _enemyId, string _prefabName, float[] _position, float _currentHealth, bool _isDead)
        {
            this.enemyId = _enemyId;
            this.prefabName = _prefabName;
            this.position = _position;
            this.currentHealth = _currentHealth;
            this.isDead = _isDead;
        }
    }
    [Serializable]
    public class LootData
    {
        public string lootId;
        public string lprefabName;
        public float[] lposition;

        public LootData(string _lootId, string _lprefabName, float[] _lposition)
        {
            this.lootId = _lootId;
            this.lprefabName= _lprefabName;
            this.lposition = _lposition;
        }
    }



    // a wrapper for UnityJson to handle arrays:
    [Serializable]
    private class SerializationWrapper<T>
    {
        public T[] items;
        public SerializationWrapper(T[] items) { this.items = items; }
    }
    #endregion

    #region SaveDataPlayFab
    public void SaveGameData()
    {

        var full = new FullSaveData
        {
            player = CreatePlayerData(),
            enemies = GatherAllEnemies(),
            loot = GatherAllLoot(),
        };
        string json = JsonUtility.ToJson(full);

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>()
            {
                { "FullGameData", json },

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
    public void LoadFullGameData(Action<FullSaveData> callback)
    {
        var req = new GetUserDataRequest { Keys = new List<string> { "FullGameData" } };
        PlayFabClientAPI.GetUserData(req, result =>
        {
            if (result.Data.TryGetValue("FullGameData", out var kv))
            {
                var full = JsonUtility.FromJson<FullSaveData>(kv.Value);
                callback?.Invoke(full);
            }
            else callback?.Invoke(null);
        }, OnDataError);
    }

    /// Creates a PlayerData object from the current game state.
    public PlayerData CreatePlayerData()
    {
        // Get player stats from PlayerState.
        float[] playerStats = new float[1];
        playerStats[0] = PlayerState.Instance.currentHealth;

        // Get player position and rotation.
        float[] posAndRot = new float[6];
        Vector3 pos = AccessPo.Instance.PlayerPosition;
        posAndRot[0] = pos.x;
        posAndRot[1] = pos.y;
        posAndRot[2] = pos.z;
        // Store forward direction for rotation.
        Vector3 forward = AccessPo.Instance.transform.forward;
        posAndRot[3] = forward.x;
        posAndRot[4] = forward.y;
        posAndRot[5] = forward.z;

        // Get inventory and quick slot content.
        string[] inventory = InventorySystem.Instance.itemList.ToArray();
        string[] quickSlots = GetQuickSlotContents();

        // Get enemy kill count and total score.
        int totalScore = GameManager.instance.totalScore;

        // Get elapsed time from GameManager.
        float remainingTime = GameManager.instance.GetRemainingTime();
        // Get current scene name.
        string currentScene = SceneManager.GetActiveScene().name;

        return new PlayerData(playerStats, posAndRot, inventory, quickSlots,totalScore, remainingTime, currentScene);
    }

    public EnemyData[] GatherAllEnemies()
    {
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        EnemyData[] result = new EnemyData[allEnemies.Length];

        for (int i = 0; i < allEnemies.Length; i++)
        {
            var e = allEnemies[i];
            string prefabName = e.gameObject.name.Replace("(Clone)", "").Trim();
            string resourcePath = "Enemies/" + prefabName;
            result[i] = new EnemyData(
                e.enemyId,
                resourcePath,
                new float[]{
                e.transform.position.x,
                e.transform.position.y,
                e.transform.position.z
                },
                e.currentHealth,
                e.isDead
            );
        }

        return result;
    }

    public LootData[] GatherAllLoot()
    {
        var all = FindObjectsOfType<LootSave>();
        var result = new LootData[all.Length];
        for (int i = 0; i < all.Length; i++)
        {
            var ls = all[i];
            var pos = ls.GetWorldPosition();
            result[i] = new LootData(
                ls.lootId,
                "Loot/" + ls.lootName,   
                new float[] { pos.x, pos.y, pos.z }
            );
        }
        return result;
    }


    /// Retrieves quick slot contents from EquipSystem.
    public string[] GetQuickSlotContents()
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
    //public IEnumerator SetGameDataCoroutine(PlayerData playerData, EnemyData[] enemies)
    //{
    //    var pm = PlayerState.Instance.playerBody
    //        .GetComponent<PlayerMovement>();
    //    if (pm != null)
    //        pm.enabled = false;
    //    var cc = PlayerState.Instance.playerBody
    //        .GetComponent<CharacterController>();
    //    if (cc != null)
    //        cc.enabled = false;
    //    // Check if the saved scene is different from the current scene.
    //    if (SceneManager.GetActiveScene().name != playerData.currentScene)
    //    {
    //        // Load the scene asynchronously.
    //        AsyncOperation op = SceneManager.LoadSceneAsync(playerData.currentScene);
    //        while (!op.isDone)
    //        {
    //            yield return null;
    //        }
                    
    //        yield return null;
    //    }
        
    //    PlayerState.Instance.currentHealth = playerData.playerStats[0];

    //    Vector3 loadPos = new Vector3(
    //        playerData.playerPositionAndRotation[0],
    //        playerData.playerPositionAndRotation[1],
    //        playerData.playerPositionAndRotation[2]
    //    );
    //    Vector3 forward = new Vector3(
    //        playerData.playerPositionAndRotation[3],
    //        playerData.playerPositionAndRotation[4],
    //        playerData.playerPositionAndRotation[5]
    //    );
    //    AccessPo.Instance.PlayerPosition = loadPos;
    //    AccessPo.Instance.PlayerRotation = Quaternion.LookRotation(forward);

    //    // Restore inventory.
    //    foreach (string item in playerData.inventoryContent)
    //    {
    //        InventorySystem.Instance.AddToInventory(item);
    //    }

    //    // Restore quick slot content.
    //    foreach (string item in playerData.quickSlotContent)
    //    {
    //        GameObject availableSlot = EquipSystem.Instance.FindNextEmptySlot();
    //        GameObject itemToAdd = Instantiate(Resources.Load<GameObject>(item));
    //        itemToAdd.transform.SetParent(availableSlot.transform, false);
    //    }

    //    // Update total score.
    //    GameManager.instance.totalScore = playerData.totalScore;

    //    // Update time.
    //    GameManager.instance.SetRemainingTime(playerData.remainingTime);

    //    foreach (var ed in enemies)
    //    {
    //        // load the prefab by name (adjust path as needed):
    //        var prefab = Resources.Load<GameObject>(ed.prefabName);
    //        if (prefab == null)
    //        {
    //            continue;
    //        }

    //        // instantiate at saved position:
    //        Vector3 pos = new Vector3(ed.position[0], ed.position[1], ed.position[2]);
    //        var go = Instantiate(prefab, pos, Quaternion.identity);

    //        // apply saved health + dead?state:
    //        var e = go.GetComponent<Enemy>();
    //        if (e == null)
    //        {
    //            continue;
    //        }

    //        e.enemyId = ed.enemyId;
    //        e.currentHealth = ed.currentHealth;
    //        if (ed.isDead)
    //        {
    //            e.ForceDieImmediate();
    //        }
    //    }

    //    Debug.Log("Player data applied.");
    //    if (pm != null) pm.enabled = true;
    //    if (cc != null) cc.enabled = true;
    //}

    public IEnumerator ApplyFullLoad(FullSaveData full)
    {
        if (SceneManager.GetActiveScene().name != full.player.currentScene)
        {
            var op = SceneManager.LoadSceneAsync(full.player.currentScene);
            while (!op.isDone) yield return null;
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        var pm = PlayerState.Instance.playerBody.GetComponent<PlayerMovement>();
        var cc = PlayerState.Instance.playerBody.GetComponent<CharacterController>();
        if (pm != null) pm.enabled = false;
        if (cc != null) cc.enabled = false;

        // player
        // — health
        PlayerState.Instance.currentHealth = full.player.playerStats[0];

        // — position & rotation
        var pd = full.player.playerPositionAndRotation;
        Vector3 playerPos = new Vector3(pd[0], pd[1], pd[2]);
        Vector3 lookFwd = new Vector3(pd[3], pd[4], pd[5]);
        AccessPo.Instance.PlayerPosition = playerPos;
        AccessPo.Instance.PlayerRotation = Quaternion.LookRotation(lookFwd);

        // — inventory
        foreach (var item in full.player.inventoryContent)
            InventorySystem.Instance.AddToInventory(item);

        // — quick-slots
        foreach (var qs in full.player.quickSlotContent)
        {
            var slot = EquipSystem.Instance.FindNextEmptySlot();
            var go = Instantiate(Resources.Load<GameObject>(qs));
            go.transform.SetParent(slot.transform, false);
        }
        // — score & timer
        GameManager.instance.totalScore = full.player.totalScore;
        GameManager.instance.SetRemainingTime(full.player.remainingTime);

        // enemies
        foreach (var ed in full.enemies)
        {
            var prefab = Resources.Load<GameObject>(ed.prefabName);
            if (prefab == null)
            {
                Debug.LogError($"Couldn't load prefab '{ed.prefabName}'");
                continue;
            }
            var spawnPos = new Vector3(ed.position[0], ed.position[1], ed.position[2]);
            var go = Instantiate(prefab, spawnPos, Quaternion.identity);
            var e = go.GetComponent<Enemy>();
            if (e == null) continue;

            // restore ID & health/death state
            e.enemyId = ed.enemyId;
            e.currentHealth = ed.currentHealth;
            if (ed.isDead) e.ForceDieImmediate();
        }
        foreach (var ld in full.loot)
        {
            // load the prefab
            var prefab = Resources.Load<GameObject>(ld.lprefabName);
            if (prefab == null)
            {
                Debug.LogError($"Couldn’t load loot prefab '{ld.lprefabName}'");
                continue;
            }

            // instantiate in world
            var spawnPos = new Vector3(ld.lposition[0], ld.lposition[1], ld.lposition[2]);
            var go = Instantiate(prefab, spawnPos, Quaternion.identity);

            // restore its save?ID so we don’t double?spawn next time
            var ls = go.GetComponent<LootSave>();
            if (ls != null)
            {
                ls.lootId = ld.lootId;
                ls.lootName = prefab.name; 
            }
        }

        // Re-enable player movement/look
        if (pm != null) pm.enabled = true;
        if (cc != null) cc.enabled = true;

        Debug.Log("Full game data applied.");
        yield break;
    }

    #endregion LoadData

    #region LeaderBoard 

    public void SendGameStatsToPlayFab()
    {
        int totalScore = GameManager.instance.totalScore;
        // Convert elapsedTime to an integer value (seconds).
        int elapsedTimeSeconds = Mathf.FloorToInt(GameManager.instance.GetRemainingTime());

        // Build the request with multiple statistics.
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = "DungeonScore", Value = totalScore }
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