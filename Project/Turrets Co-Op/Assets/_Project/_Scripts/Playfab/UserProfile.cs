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
    void GetUserData () { //Calls get user data on useraccmanager to get data according to a key from playfab api
        UserAccManager.instance.GetUserData ("ProfileData");
    }
    
    void UserDataRetrieved(string key, string value)
    {
        if (key == "ProfileData" && value != null)
        {
            profileData = JsonUtility.FromJson<ProfileData>(value);
            OnProfileDataUpdated.Invoke(profileData);
        }
        else
        {
            // Create a new blank profile data and generate a random username
            profileData = new ProfileData();

            // Call API_Manager to generate a random username
            API_Manager.instance.UsernameGen(UserAccManager.instance.emailAddress, 5, (generatedUsername) => 
            {
                if (!string.IsNullOrEmpty(generatedUsername))
                {
                    profileData.name = generatedUsername;
                    Debug.Log("Generated Username: " + generatedUsername);

                    // Save the new profile data with the generated username
                    SetUserData(() =>
                    {
                        OnProfileDataUpdated.Invoke(profileData); // Notify listeners of the updated profile data
                    });
                }
                else
                {
                    Debug.Log("Failed to generate a random username.");
                }
            });
        }
    }
    
    void SetUserData (UnityAction OnSuccess = null) { //sets user data according to playfab key and profiledata class
        UserAccManager.instance.SetUserData ("ProfileData", JsonUtility.ToJson (profileData), OnSuccess);
    }

    public int GetCurrentHigh () { //used to retrieve current highscore
        return (int)profileData.highScore;
    }
    
    
    public void SetHigh (float newHigh) { //sets new highscore and it's stat on scoreboard
        profileData.highScore = newHigh;
        SetUserData (GetUserData);
        UserAccManager.instance.SetStat ("highScore", Mathf.FloorToInt (profileData.highScore));
    }

    public void SetPlayerName (string playerName) { //sets new player display name
        profileData.name = playerName;
            SetUserData (GetUserData);
            UserAccManager.instance.SetDisplayName (playerName);
    }

    void GetLeaderBoardHigh () { //retrieves leaderboard
        UserAccManager.instance.GetLeaderBoard ("highScore");
    }
    
    void LeaderboardRetrieved (string key, List<PlayerLeaderboardEntry> leaderboardEntries) { ///leaderboard has been retrieved invoke event
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
