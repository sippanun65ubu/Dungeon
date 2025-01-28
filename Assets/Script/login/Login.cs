using System.Collections;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour
{
    //firebase varialbes
    [SerializeField] Text messagetext;

    //firebase varialbes
    [Header("Login")]
    [SerializeField] InputField loginEmail;
    [SerializeField] InputField loginPassword;
    [SerializeField] GameObject loginpage;

    //firebase varialbes
    [Header("Signup")]  
    [SerializeField] InputField signupUsername;
    [SerializeField] InputField signupEmail;
    [SerializeField] InputField signupPassword;
    [SerializeField] InputField signupCPassword;
    [SerializeField] GameObject signuppage;

    [Header("forgetpassword")]
    [SerializeField] GameObject forgetpasswordpage;
    [SerializeField] InputField forgetpasswordEmail;


    [Header("Menu")]
    [SerializeField] GameObject menupage;


    private void Start()
    {
        
    }
    private void Update()
    {
        
    }

    public void ClearScreen()
    {
        loginpage.SetActive(false);
        signuppage.SetActive(false);
        menupage.SetActive(false);
        forgetpasswordpage.SetActive(false);
    }
    public void LoginScreen() //Back button
    {
        ClearScreen();
        ClearLoginFeilds();
        loginpage.SetActive(true);
    }
    public void RegisterScreen() // Regester button
    {
        ClearScreen();
        ClearSignUpFeilds();
        signuppage.SetActive(true);
    }
    public void MainmenuScreen()
    {
        ClearScreen();
        menupage.SetActive(true);
    }

    public void ForgetPasswordScreen()
    {
        ClearScreen();
        ClearRecoveryFeilds();
        forgetpasswordpage.SetActive(true);
    }
    //Function for the login button
    public void ClearLoginFeilds()
    {
        loginEmail.text = "";
        loginPassword.text = "";
        messagetext.text = "";
    }
    public void ClearRecoveryFeilds()
    {
        forgetpasswordEmail.text = "";
        messagetext.text = "";
    }

    public void ClearSignUpFeilds()
    {
        signupEmail.text = "";
        signupUsername.text = "";
        signupPassword.text = "";
        signupCPassword.text = "";
        messagetext.text = "";
    }
    public void SignOutButton()
    {
        ClearSignUpFeilds();
        ClearLoginFeilds();
    }
    public void RegisterUser()
    {
        //if statement  if password is less than 6 message text = too short password

        var request = new RegisterPlayFabUserRequest
        {
            DisplayName = signupUsername.text,
            Email = signupEmail.text,
            Password = signupPassword.text,

            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnregisterSuccess, OnError);
    }

    private void OnError(PlayFabError Error)
    {
        messagetext.text = Error.ErrorMessage;
        Debug.Log(Error.GenerateErrorReport());
    }

    private void OnregisterSuccess(RegisterPlayFabUserResult result)
    {
        messagetext.text = "New Account Is Created";
        LoginScreen();
    }
    
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
        messagetext.text = "Logged In";
        MainmenuScreen();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);   
    }

    public void RecoverUser()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = forgetpasswordEmail.text,
            TitleId = "48A9A"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnRecoverySucces, OnErrorRecovery);
    }

    private void OnErrorRecovery(PlayFabError result)
    {
        messagetext.text = "No Email Found";
    }

    private void OnRecoverySucces(SendAccountRecoveryEmailResult result)
    {
        messagetext.text = "Recovery Mail Sent";
        LoginScreen();
    }

    public void Startgame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}