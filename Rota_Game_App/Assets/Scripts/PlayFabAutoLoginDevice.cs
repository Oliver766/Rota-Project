using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabAutoLoginDevice : MonoBehaviour
{

    void Start()
    {
        login();
    }
    void login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login successful!");
    }
    
    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login failed: " + error.GenerateErrorReport());
    }
}