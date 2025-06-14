using System.Collections;
using System.Collections.Generic;
using PlayFab;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirstMenuManager : MonoBehaviour
{
    //      UI & Login Section   //
    [Header("Login UI")]
    public InputField loginEmail;
    public InputField loginPassword;
    public GameObject loginPage;

    [Header("Signup UI")]
    public InputField signupUsername;
    public InputField signupEmail;
    public InputField signupPassword;
    public InputField signupCPassword;
    public GameObject signupPage;

    [Header("Forget Password UI")]
    public InputField forgetPasswordEmail;
    public GameObject forgetPasswordPage;

    [Header("Main Menu UI")]
    public GameObject menuPage;

    [Header("Leaderboard UI")]
    public GameObject leaderboardPage;
    public GameObject KeyInfoMenu;
    public Text messageText;

    public static FirstMenuManager Instance { get; set; }
    private void Awake()
    {
        // Setup singleton.
        if (Instance == null)
        {
            Instance = this;
           
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void NewGame()
    {

        SceneManager.LoadScene("TownNo2");
    }
    public void ClearScreen()
    {
        loginPage.SetActive(false);
        signupPage.SetActive(false);
        menuPage.SetActive(false);
        forgetPasswordPage.SetActive(false);
        leaderboardPage.SetActive(false);
        KeyInfoMenu.SetActive(false);
        ClearLoginFields();
        ClearSignUpFields();
        ClearRecoveryFields();
    }
    public void LoginScreen()
    {
        ClearScreen();
        PlayFabClientAPI.ForgetAllCredentials();
        loginPage.SetActive(true);
    }
    public void RegisterScreen()
    {
        ClearScreen();
        signupPage.SetActive(true);
    }
    public void MainMenuScreen()
    {
        ClearScreen();
        menuPage.SetActive(true);
    }
    public void ForgetPasswordScreen()
    {
        ClearScreen();
        forgetPasswordPage.SetActive(true);
    }
    public void LeaderBoardScreen()
    {
        ClearScreen();
        leaderboardPage.SetActive(true);
    }
    public void KeyInfoScreen()
    {
        ClearScreen();
        KeyInfoMenu.SetActive(true);
    }
    public void ClearLoginFields()
    {
        loginEmail.text = "";
        loginPassword.text = "";
        messageText.text = "";
    }
    public void ClearRecoveryFields()
    {
        forgetPasswordEmail.text = "";
        messageText.text = "";
    }
    public void ClearSignUpFields()
    {
        signupEmail.text = "";
        signupUsername.text = "";
        signupPassword.text = "";
        signupCPassword.text = "";
        messageText.text = "";
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void LoadGame()
    {
        if (PLayFabManager.Instance == null)
        {
            Debug.LogError("PlayFab manager not found!");
            return;
        }

        PLayFabManager.Instance.LoadFullGameData(full =>
        {
            if (full == null)
            {
                Debug.LogWarning("No saved game data found.");
                // Optional: show “No save” popup
                return;
            }

            // Start the coroutine that rehydrates player, enemies, quests, NPCs…
            PLayFabManager.Instance.StartCoroutine(
                PLayFabManager.Instance.ApplyFullLoad(full)
            );
        });
    }
}
