using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;

public class GoogleManager : MonoBehaviour
{
    public TextMeshProUGUI logText;

    void Start()
    {
        Debug.Log("App Starting...");
        Debug.Log("Enabling PlayGamesPlatform debug log.");
        //PlayGamesPlatform.DebugLogEnabled = true;

        Debug.Log("Activating PlayGamesPlatform.");
        //PlayGamesPlatform.Activate();

        Debug.Log("Calling SignIn...");
        SignIn();
        
    }

    public void SignIn()
    {
        Debug.Log("Starting Authentication...");
        //PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication, true);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        Debug.Log("Authentication Status: " + status);

        switch (status)
        {
            case SignInStatus.Success:
                Debug.Log("Login successful.");
                //string name = PlayGamesPlatform.Instance.GetUserDisplayName();
                //string id = PlayGamesPlatform.Instance.GetUserId();
                //string ImgUrl = PlayGamesPlatform.Instance.GetUserImageUrl();
                logText.text = "Success \n" + name;
                break;

            case SignInStatus.Canceled:
                Debug.LogError("Login canceled by the user.");
                logText.text = "Sign in Failed! Canceled by the user.";
                break;

            case SignInStatus.InternalError:
                Debug.LogError("Internal error occurred during login.");
                logText.text = "Sign in Failed! Internal error.";
                break;

            default:
                Debug.LogError("Unknown error: " + status);
                logText.text = "Sign in Failed! Unknown error.";
                break;
        }
    }
}