using System;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.Events;

public class UserAccManager : MonoBehaviour {

    public static UserAccManager instance;

    public static UnityEvent OnLoginSuccess = new UnityEvent ();
    public static UnityEvent<string> OnLoginFailed = new UnityEvent<string> ();
    
    public static UnityEvent<string> OnCreateAccountFailed = new UnityEvent<string> ();

    void Awake () {
        instance = this;
    }

    public void CreatAccount (string username, string emailAdress, string password) {
        PlayFabClientAPI.RegisterPlayFabUser (
            new RegisterPlayFabUserRequest() {
                Email = emailAdress,
                Password = password,
                Username = username
            },
            response => {
                Debug.Log ($"Successful Account Creation: {username}, {emailAdress}");
                SignIn (username, password);
            },
            error => {
                Debug.Log ($"Unsuccessful Account Creation: {username}, {emailAdress} \n {error.ErrorMessage}");
                OnCreateAccountFailed.Invoke (error.ErrorMessage);
            }
        );
    }

    public void SignIn (string username, string password) {
        PlayFabClientAPI.LoginWithPlayFab (new LoginWithPlayFabRequest () {
                Username = username,
                Password = password
            },
            response => {
                Debug.Log ($"Successful Account Login: {username}");
                OnLoginSuccess.Invoke ();
            },
            error => {
                Debug.Log ($"Unsuccessful Account Login: {username} \n {error.ErrorMessage}");
                OnLoginFailed.Invoke (error.ErrorMessage);
            });
    }
}
