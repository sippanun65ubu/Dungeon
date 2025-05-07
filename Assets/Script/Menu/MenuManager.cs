using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; set; }

    public GameObject menuCanvas;
    public GameObject uiCanvas;
    public GameObject saveMenu;
    public GameObject settingMenu;
    public GameObject newOrLoadMenu;
    public static bool showNewOrLoadOnStart = true;
    public bool isMenuOpen;

    private void Awake()
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
    public void Start()
    {
        menuCanvas.SetActive(true);
        uiCanvas.SetActive(false);
        newOrLoadMenu.SetActive(true);

        settingMenu.SetActive(false);
        saveMenu.SetActive(false);

        showNewOrLoadOnStart = false;
        isMenuOpen = true;
        GameManager.instance.Pause();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SelectionManager.Instance.DisableSelection();
        SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;
        MovementManager.instance.EnableLook(false);
        MovementManager.instance.EnableMovement(false);
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !isMenuOpen)
        {
            saveMenu.SetActive(false);
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

            saveMenu.SetActive(false);
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
            saveMenu.SetActive(false);
            menuCanvas.SetActive(false);
            newOrLoadMenu.SetActive(false);
        }
    }
    public void GoToSaving()
    {
        if (isMenuOpen == true)
        {
            settingMenu.SetActive(false);
            saveMenu.SetActive(true);
            menuCanvas.SetActive(false);
            newOrLoadMenu.SetActive(false);
        }
    }
    public void GoToingamemanu()
    {
        if (isMenuOpen == true)
        {
            settingMenu.SetActive(false);
            saveMenu.SetActive(false);
            menuCanvas.SetActive(true);
            newOrLoadMenu.SetActive(false);
        }
    }
    public void Mainmennu()
    {
        if (isMenuOpen == true)
        {
            settingMenu.SetActive(false);
            saveMenu.SetActive(false);
            menuCanvas.SetActive(false);
            newOrLoadMenu.SetActive(false);
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void Loadmenu()
    {
        if (isMenuOpen == true)
        {
            settingMenu.SetActive(false);
            saveMenu.SetActive(false);
            menuCanvas.SetActive(false);
            newOrLoadMenu.SetActive(true);
        }
    }
    public IEnumerator ClosesMenu()
    {
        if (isMenuOpen == true)
        {
            yield return new WaitForSeconds(1f); // wait 1 second (or your custom coroutine)

            saveMenu.SetActive(false);
            settingMenu.SetActive(false);
            newOrLoadMenu.SetActive(false);

            uiCanvas.SetActive(true);
            menuCanvas.SetActive(false);
            GameManager.instance.UnpauseGame();
            isMenuOpen = false;

            MovementManager.instance.EnableLook(true);
            MovementManager.instance.EnableMovement(true);

            if (!InventorySystem.Instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            SelectionManager.Instance.EnableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
        }
    }
}
