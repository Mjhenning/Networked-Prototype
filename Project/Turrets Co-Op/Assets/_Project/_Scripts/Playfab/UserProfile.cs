using System;
using UnityEngine;
using UnityEngine.Events;

public class UserProfile : MonoBehaviour {

    public static UserProfile instance;

    public static UnityEvent<ProfileData> OnProfileDataUpdated = new UnityEvent<ProfileData> (); 
    [SerializeField] ProfileData profileData;
    
    

    void Awake () {
        instance = this;
    }

    void OnEnable () {
        UserAccManager.OnLoginSuccess.AddListener (SignIn);
        UserAccManager.OnUserDataRetrieved.AddListener (UserDataRetrieved);
    }
    
    void OnDisable () {
        UserAccManager.OnLoginSuccess.RemoveListener(SignIn);
        UserAccManager.OnUserDataRetrieved.RemoveListener (UserDataRetrieved);
    }
    
    void SignIn () {
        GetUserData ();
    }

    [ContextMenu("Get Profile Data")]
    void GetUserData () {
        UserAccManager.instance.GetUserData ("ProfileData");
    }
    
    void UserDataRetrieved (string key, string value) {
        if (key == "ProfileData") {
            profileData = JsonUtility.FromJson<ProfileData> (value);
            OnProfileDataUpdated.Invoke (profileData);
        }
    }

    [ContextMenu("Set Profile Data")]
    void SetUserData (UnityAction OnSuccess = null) {
        UserAccManager.instance.SetUserData ("ProfileData", JsonUtility.ToJson (profileData), OnSuccess);
    }

    public void ChangeScore (float newHigh) {
        profileData.highScore = newHigh;
        SetUserData (GetUserData);
    }

    public void SetPlayerName (string playerName) {
        profileData.name = playerName;
        SetUserData (GetUserData);
    }

}

[System.Serializable]
public class ProfileData { //data format of player data
    public string name;
    public float highScore;
}
