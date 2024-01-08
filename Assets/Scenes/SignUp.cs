using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Extensions;

public class SignUp : MonoBehaviour
{
    public TMP_InputField userNameField;
    public TMP_InputField passwordField;
    public TMP_InputField confirmPasswordField;
    public TMP_InputField emailField;
    public Button loginButton;
    public TMP_Text errorText; // New UI element to display errors

    private FirebaseAuth auth;
    private FirebaseUser user;

    void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    void SignUpUser()
    {
        string userName = userNameField.text;
        string email = emailField.text;
        string password = passwordField.text;
        string confirmPassword = confirmPasswordField.text;

        if (ValidatePassword(password, confirmPassword))
        {

            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    UpdateErrorText("Sign-up canceled"); // Display error message
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    UpdateErrorText("Sign-up failed"); // Display error message
                    return;
                }
                UpdateErrorText("Sign up succesfull"); // Display error message
                Firebase.Auth.AuthResult result = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
            });
        }
        else
        {
            UpdateErrorText("Passwords do not match"); // Display error message
            return;
        }
    }

    bool ValidatePassword(string password, string confirmPassword)
    {
        return password == confirmPassword;
    }

    void UpdateErrorText(string errorMessage)
    {
        errorText.text = errorMessage;
    }
}
