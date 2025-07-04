using System.Collections;
using System.Collections.Generic;
using PlayFab;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; set; }

    public GameObject menuCanvas;
    public GameObject uiCanvas;
    public GameObject settingMenu;
    public bool isMenuOpen;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !isMenuOpen)
        {
            settingMenu.SetActive(false);

            uiCanvas.SetActive(false);
            menuCanvas.SetActive(true);

            isMenuOpen = true;
            GameManager.instance.Pause();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SelectionManager.Instance.DisableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;
            MovementManager.instance.EnableLook(false);
            MovementManager.instance.EnableMovement(false);
        }
        else if (Input.GetKeyUp(KeyCode.Escape) && isMenuOpen)
        {
            settingMenu.SetActive(false);

            uiCanvas.SetActive(true);
            menuCanvas.SetActive(false);
            GameManager.instance.UnpauseGame();
            isMenuOpen = false;
            MovementManager.instance.EnableLook(true);
            MovementManager.instance.EnableMovement(true);

            if (InventorySystem.Instance.isOpen == false)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            SelectionManager.Instance.EnableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
        }
    }
    public void GoToSetting()
    {
        if (isMenuOpen == true)
        {
            settingMenu.SetActive(true);
            menuCanvas.SetActive(false);
        }
    }
    public void GoToingamemanu()
    {
        if (isMenuOpen == true)
        {
            settingMenu.SetActive(false);
            menuCanvas.SetActive(true);
        }
    }
    public void Mainmenu()
    {
        if (isMenuOpen == true)
        {
            if (PLayFabManager.Instance != null)
                PLayFabManager.Instance.SaveGameData();
            else
                Debug.LogWarning("No PlayFabManager – skipping SaveGameData()");


            if (PlayerState.Instance != null) PlayerState.Instance.ResetToDefaults();
            if (GameManager.instance != null) GameManager.instance.ResetToDefaults();
            if (InventorySystem.Instance != null) InventorySystem.Instance.ClearAllItems();
            if (EquipSystem.Instance != null) EquipSystem.Instance.ResetToDefaults();
            if (QuestManager.instance != null) QuestManager.instance.ResetToDefaults();

            // 3) Reset every spawn‐point
            foreach (var sp in FindObjectsOfType<EnemySpawnPoint>())
                sp.ResetSpawnedEnemies();
            foreach (var sp in FindObjectsOfType<EnemySpawnerNearPlayer>())
                sp.KillAllEnemies();
            // 4) Reset every NPC
            foreach (var npc in FindObjectsOfType<NPC>())
                npc.ResetToDefaults();
            PlayFabClientAPI.ForgetAllCredentials();
            settingMenu?.SetActive(false);
            menuCanvas?.SetActive(false);
            SceneManager.LoadScene("MainMenu");
            Application.Quit();
        }
    }

    public void ClosesMenu()
    {
        if (isMenuOpen == true)
        {
            settingMenu.SetActive(false);

            uiCanvas.SetActive(true);
            menuCanvas.SetActive(false);
            GameManager.instance.UnpauseGame();
            isMenuOpen = false;
            MovementManager.instance.EnableLook(true);
            MovementManager.instance.EnableMovement(true);

            if (InventorySystem.Instance.isOpen == false)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            SelectionManager.Instance.EnableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
        }
    }
    
}
