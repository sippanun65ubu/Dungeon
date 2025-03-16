using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirstMenuManager : MonoBehaviour
{
    //      UI & Login Section   //
    [Header("Login UI")]
    [SerializeField] InputField loginEmail;
    [SerializeField] InputField loginPassword;
    [SerializeField] GameObject loginPage;

    [Header("Signup UI")]
    [SerializeField] InputField signupUsername;
    [SerializeField] InputField signupEmail;
    [SerializeField] InputField signupPassword;
    [SerializeField] InputField signupCPassword;
    [SerializeField] GameObject signupPage;

    [Header("Forget Password UI")]
    [SerializeField] InputField forgetPasswordEmail;
    [SerializeField] GameObject forgetPasswordPage;

    [Header("Main Menu UI")]
    [SerializeField] GameObject menuPage;

    [Header("Leaderboard UI")]
    [SerializeField] GameObject leaderboardPage;
    [SerializeField] public GameObject KeyInfoMenu;
    [SerializeField] Text messageText;

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

        SceneManager.LoadScene("Town");
    }
    public void ExitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
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
}
