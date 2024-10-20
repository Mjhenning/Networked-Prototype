using System;
using TMPro;
using UnityEngine;

public class UIProfile : MonoBehaviour {
    [SerializeField] TMP_InputField playerNameText;
    [SerializeField] TextMeshProUGUI playerHighestScoreText;

    void OnEnable () {
        UserAccManager.OnLoginSuccess.AddListener (Login);
        UserProfile.OnProfileDataUpdated.AddListener (ProfileDataUpdated);
    }
    
    void OnDisable () {
        UserAccManager.OnLoginSuccess.RemoveListener (Login);
        UserProfile.OnProfileDataUpdated.RemoveListener (ProfileDataUpdated);
    }
    
    void Login () {
        GetComponent<CanvasGroup> ().alpha = 1;
    }

    void ProfileDataUpdated (ProfileData profileData) {
        if (profileData != null) {
            if (profileData.name != null) playerNameText.text = profileData.name;
            if (profileData.highScore != 0) playerHighestScoreText.text = Mathf.Floor(profileData.highScore).ToString();
        }
    }
    
}
