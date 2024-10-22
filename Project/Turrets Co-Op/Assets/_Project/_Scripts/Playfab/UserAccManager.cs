using System;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.Events;

public class UserAccManager : MonoBehaviour { //Overall Playfab Manager
    public static UserAccManager instance;

    public static UnityEvent OnLoginSuccess = new UnityEvent ();
    public static UnityEvent<string> OnLoginFailed = new UnityEvent<string> ();
    public static UnityEvent<string> OnCreateAccountFailed = new UnityEvent<string> ();
    public static UnityEvent<string, string> OnUserDataRetrieved = new UnityEvent<string, string> ();
    public static UnityEvent<string, int> OnStatRetrieved = new UnityEvent<string, int> ();
    public static UnityEvent<string, List<PlayerLeaderboardEntry>> OnLeaderBoardRetrieved = new UnityEvent<string, List<PlayerLeaderboardEntry>> ();

    public string playfabID;

    void Awake () {
        instance = this;
    }

    public void CreatAccount (string username, string emailAdress, string password) { //used to create a new account on playfab
        PlayFabClientAPI.RegisterPlayFabUser (
            new RegisterPlayFabUserRequest () {
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
        PlayFabClientAPI.LoginWithPlayFab (new LoginWithPlayFabRequest () { //used to login to a created account / login to an existing account
                Username = username,
                Password = password
            },
            response => {
                Debug.Log ($"Successful Account Login: {username}");
                OnLoginSuccess.Invoke ();
                playfabID = response.PlayFabId;
            },
            error => {
                Debug.Log ($"Unsuccessful Account Login: {username} \n {error.ErrorMessage}");
                OnLoginFailed.Invoke (error.ErrorMessage);
            });
    }
    
    //USERDATA

    public void GetUserData (string key) { //used to retrieve a logged in user's data
        PlayFabClientAPI.GetUserData (new GetUserDataRequest () {
            PlayFabId = playfabID,
            Keys = new List<string>() {
                key
            }
        }, response => {
            Debug.Log ($"Successful GetUserData");
            if (response.Data.ContainsKey (key)) OnUserDataRetrieved.Invoke (key, response.Data[key].Value);
            else OnUserDataRetrieved.Invoke (key, null);
        }, error => {
            Debug.Log ($"Unsuccessful GetUserData: {error.ErrorMessage}");
        });
    }

    public void SetUserData (string key, string value, UnityAction OnSuccess = null) {
        PlayFabClientAPI.UpdateUserData (new UpdateUserDataRequest () {
            Data = new Dictionary<string, string>() {
                {key, value}
            }
        }, response => {
            Debug.Log ($"Successful SetUserData");
            OnSuccess.Invoke();
        }, error => {
            Debug.Log ($"Unsuccessful SetUserData: {error.ErrorMessage}");
        });
    }

    public void SetDisplayName (string displayname) {
        if (displayname != null && displayname != "") {
            PlayFabClientAPI.UpdateUserTitleDisplayName (new UpdateUserTitleDisplayNameRequest () {
                DisplayName = displayname
            }, response => {
                Debug.Log ($"Successful SetDisplayName");
            }, error => {
                Debug.Log ($"Unsuccessful SetDisplayName: {error.ErrorMessage}");
            }); 
        }
    }
    
    //STATS & LEADERBOARDS

    public void GetStat (string key) {
        PlayFabClientAPI.GetPlayerStatistics (new GetPlayerStatisticsRequest () {
            StatisticNames = new List<string>(){key}
        }, response => {
            Debug.Log ($"Successful GetStat");
            if (response.Statistics != null && response.Statistics.Count > 0) 
                OnStatRetrieved.Invoke (key, response.Statistics[0].Value);
        }, error => {
            Debug.Log ($"Unsuccessful GetStat : {error.ErrorMessage}");
        });
    }

    public void SetStat (string key, int value, UnityAction OnSuccess = null) {
        PlayFabClientAPI.UpdatePlayerStatistics (new UpdatePlayerStatisticsRequest () {
            Statistics = new List<StatisticUpdate>() {
                new StatisticUpdate() {
                    StatisticName = key,
                    Value = value
                }
            }
        }, response => {
            Debug.Log ($"Successful SetStat");
        }, error => {
            Debug.Log ($"Unsuccessful SetStat : {error.ErrorMessage}");
        });
    }

    public void GetLeaderBoard (string key) {
        PlayFabClientAPI.GetLeaderboard (new GetLeaderboardRequest () {
            StatisticName = key,
            MaxResultsCount = 30 
        }, response => {
            Debug.Log ($"Successful GetLeaderBoard");
            if (response.Leaderboard != null) OnLeaderBoardRetrieved.Invoke (key, response.Leaderboard);
        }, error => {
            Debug.Log ($"Unsuccessful GetLeaderBoard : {error.ErrorMessage}");
        });
    }
    
}