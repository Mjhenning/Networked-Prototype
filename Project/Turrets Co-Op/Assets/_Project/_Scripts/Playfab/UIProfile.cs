using System;
using TMPro;
using UnityEngine;

public class UIProfile : MonoBehaviour {
    [SerializeField] public TMP_InputField playerNameText;
    [SerializeField] TextMeshProUGUI playerHighestScoreText;
    [SerializeField] TextMeshProUGUI errorText;

    public static UIProfile instance;
    
    void OnEnable () {
        UserAccManager.OnLoginSuccess.AddListener (Login);
        UserProfile.OnProfileDataUpdated.AddListener (ProfileDataUpdated);
    }
    
    void OnDisable () {
        UserAccManager.OnLoginSuccess.RemoveListener (Login);
        UserProfile.OnProfileDataUpdated.RemoveListener (ProfileDataUpdated);
    }

    void Awake () {
        instance = this;
    }

    void Login () { //sets login screen alpha to 1
        GetComponent<CanvasGroup> ().alpha = 1;
    }

    void ProfileDataUpdated (ProfileData profileData) { //once pprofile data has been updated set text fields according to retrieved data
        if (profileData != null) {
            if (profileData.name != null) playerNameText.text = profileData.name;
            if (profileData.highScore != 0) playerHighestScoreText.text = Mathf.Floor(profileData.highScore).ToString();
        }
    }

    public void DisplayErrorMessage () { //used to enable error message when display name text field is empty
        errorText.gameObject.SetActive (true);
    }

    public void HideErrorMessage () { //used to hide error message
        if (errorText.gameObject.activeSelf) {
            errorText.gameObject.SetActive (false);
        }
    }
    
}
