using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using Firebase;


public class Login : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public Button loginButton;
    public Button goToRegisterButton;
    public TextMeshProUGUI errorText; // New UI element to display errors'
    
    

    private FirebaseAuth auth;
    private FirebaseUser user;

    ArrayList credentials;

    void Start()
    {
        errorText.text = "";
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            FirebaseApp app = FirebaseApp.DefaultInstance;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });

        if (File.Exists(Application.dataPath + "/credentials.txt"))
        {
            credentials = new ArrayList(File.ReadAllLines(Application.dataPath + "/credentials.txt"));
        }
        else
        {
            Debug.Log("Credential file doesn't exist");
        }
    }

    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null && auth.CurrentUser.IsValid();
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
               Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    void UpdateErrorText(string errorMessage)
    {
        errorText.text = errorMessage;
        Debug.Log($"Error message updated: {errorMessage}");
    }

    void login()
    {
        bool isExists = false;

        credentials = new ArrayList(File.ReadAllLines(Application.dataPath + "/credentials.txt"));

        foreach (var i in credentials)
        {
            string line = i.ToString();
            if (i.ToString().Substring(0, i.ToString().IndexOf(":")).Equals(usernameInput.text) &&
                i.ToString().Substring(i.ToString().IndexOf(":") + 1).Equals(passwordInput.text))
            {
                isExists = true;
                break;
            }
        }

        if (isExists)
        {
            //Debug.Log($"Logging in '{usernameInput.text}'");
            // Clear any previous error messages
            UpdateErrorText("");
            loadWelcomeScreen();
        }
        else
        {
            Debug.Log("Incorrect credentials");
            errorText.text = "Incorrect credentials";
            UpdateErrorText("Incorrect credentials"); // Display error message
        }
    }

    public void moveToRegister()
    {
        SceneManager.LoadScene("Register");
    }

    void loadWelcomeScreen()
    {
        // Clear any previous error messages
        UpdateErrorText("");
        // Navigate to the home screen
        SceneManager.LoadScene("WelcomeScreen");
    }

    public void loginFirebase()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;


        if(username == "")
        {
            UpdateErrorText("username feild empty");
            return;
        }

        if (password == "")
        {
            UpdateErrorText("password feild empty");
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(username, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                //Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                UpdateErrorText("password or username invalid");
                errorText.text = "password or username invalid";
                return;
            }
            if (task.IsFaulted)
            {
                //Debug.LogError("User signed in failed");
                UpdateErrorText("password or username invalid");
                errorText.text = "password or username invalid";
                return;
            }
            errorText.text = "login succesfull";
            Firebase.Auth.AuthResult result = task.Result;
            //Debug.LogFormat("User signed in successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
        });

    }

}
