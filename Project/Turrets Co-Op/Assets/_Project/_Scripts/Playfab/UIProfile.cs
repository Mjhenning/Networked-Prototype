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

    void Login () {
        GetComponent<CanvasGroup> ().alpha = 1;
    }

    void ProfileDataUpdated (ProfileData profileData) {
        if (profileData != null) {
            if (profileData.name != null) playerNameText.text = profileData.name;
            if (profileData.highScore != 0) playerHighestScoreText.text = Mathf.Floor(profileData.highScore).ToString();
        }
    }

    public void DisplayErrorMessage () {
        errorText.gameObject.SetActive (true);
    }

    public void HideErrorMessage () {
        if (errorText.gameObject.activeSelf) {
            errorText.gameObject.SetActive (false);
        }
    }
    
}
