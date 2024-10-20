using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Events;

public class UserProfile : MonoBehaviour {

    public static UserProfile instance;

    public static UnityEvent<ProfileData> OnProfileDataUpdated = new UnityEvent<ProfileData> ();
    
    public static UnityEvent<List<PlayFab.ClientModels.PlayerLeaderboardEntry>> OnLeaderBoardHighUpdated = new UnityEvent<List<PlayFab.ClientModels.PlayerLeaderboardEntry>> (); 
    
    [SerializeField] ProfileData profileData;
    

    void Awake () {
        instance = this;
    }

    void OnEnable () {
        UserAccManager.OnLoginSuccess.AddListener (SignIn);
        UserAccManager.OnUserDataRetrieved.AddListener (UserDataRetrieved);
        UserAccManager.OnLeaderBoardRetrieved.AddListener (LeaderboardRetrieved);
    }

    void OnDisable () {
        UserAccManager.OnLoginSuccess.RemoveListener(SignIn);
        UserAccManager.OnUserDataRetrieved.RemoveListener (UserDataRetrieved);
        UserAccManager.OnLeaderBoardRetrieved.RemoveListener (LeaderboardRetrieved);
    }
    
    void SignIn () {
        GetUserData ();
        GetLeaderBoardHigh ();
    }

    [ContextMenu("Get Profile Data")]
    void GetUserData () {
        UserAccManager.instance.GetUserData ("ProfileData");
    }
    
    void UserDataRetrieved (string key, string value) {
        if (key == "ProfileData" && value != null) {
            profileData = JsonUtility.FromJson<ProfileData> (value);
            OnProfileDataUpdated.Invoke (profileData);
        } else { //create a new blank profile data that can be edited
            profileData = new ProfileData ();
            OnProfileDataUpdated.Invoke (profileData);
        }
    }
    
    void SetUserData (UnityAction OnSuccess = null) {
        UserAccManager.instance.SetUserData ("ProfileData", JsonUtility.ToJson (profileData), OnSuccess);
    }

    public int GetCurrentHigh () { //used to retrieve current highscore
        return (int)profileData.highScore;
    }
    
    
    public void ChangeScore (float newHigh) {
        profileData.highScore = newHigh;
        SetUserData (GetUserData);
        UserAccManager.instance.SetStat ("highScore", Mathf.FloorToInt (profileData.highScore));
    }

    public void SetPlayerName (string playerName) {
        profileData.name = playerName;
            SetUserData (GetUserData);
            UserAccManager.instance.SetDisplayName (playerName);
    }

    void GetLeaderBoardHigh () {
        UserAccManager.instance.GetLeaderBoard ("highScore");
    }
    
    void LeaderboardRetrieved (string key, List<PlayerLeaderboardEntry> leaderboardEntries) {
        if (key == "highScore") {
            OnLeaderBoardHighUpdated.Invoke (leaderboardEntries);
        }
    }
}

[System.Serializable]
public class ProfileData { //data format of player data
    public string name;
    public float highScore;
}
