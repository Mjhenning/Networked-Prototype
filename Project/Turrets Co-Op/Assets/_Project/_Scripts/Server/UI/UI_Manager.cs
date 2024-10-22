using System;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour { 
    
    public static UI_Manager instance;

    [SerializeField] GameObject hostRetryBtn;
    [SerializeField] GameObject hostStartBtn;
    [SerializeField] TMP_Text scoreText; // Reference to the UI Text component
    [SerializeField] TMP_Text timerText;

    [SerializeField] GameObject endScreen;
    [SerializeField] TMP_Text endScoreTxt;
    [SerializeField] TMP_Text personalEndScoreTxt;

    [SerializeField] List<Text> dcBtnText = new List<Text> ();

    void Awake () {
        instance = this;
    }
    
    void Start () {

        if (Game_Manager.instance) {
            Game_Manager.instance.endGame.AddListener (ToggleGameOver);
        }

        if (PlayerManager.instance) {
            PlayerManager.instance.listChanged.AddListener (UpdateBtnText);
        }
    }

    [Client]
    public void ToggleButtonVisibility () {

        Debug.Log ("Toggling button visibility " + "Current player count is " + PlayerManager.instance.playersList.Count);
        
        if (PlayerManager.instance.playersList.Count > 1) return;
        
        //hostRetryBtn.SetActive (!hostRetryBtn.activeSelf);
        ToggleStartBtn ();
    }

    [Client]
    public void ToggleStartBtn () {
        hostStartBtn.SetActive (!hostStartBtn.activeSelf); 
    }

    [Client]
    void ToggleGameOver () { //enables game over UI
        endScreen.SetActive (!endScreen.activeSelf);
        scoreText.gameObject.SetActive (false);
    }
    
    [Client]
    public void UpdatePersonalScoreText (int amount) { //Updates game over ui personal score text 
        if (personalEndScoreTxt) {
            personalEndScoreTxt.text = "Personal: " + Environment.NewLine + amount;
        }
    }

    [Client]
    public void UpdateHealth (bool add) {

        switch (add) {
            case false:
                HealthManager.instance.RemoveAHeart ();
                break;
        }
    }

    [Client]
    public void ResetHealth () {
        HealthManager.instance.ResetHearts ();
    }

    [Client]
    public void ChangeHeartsColor (Color color) {
        HealthManager.instance.ChangeColor (color);
    }

    [Client]
    public void UpdateScoreText(int newScore) //updates text of scores client side
    {
        if (scoreText)
        {
            scoreText.text = "Score: " + newScore;
        }

        if (endScoreTxt) {
            endScoreTxt.text = "Overall Score: " + Environment.NewLine + newScore;
        }
    }

    [Client]
    public void UpdateTimerText (string time) { //updates timer text client side
        if (timerText) {
            timerText.text = time;
        }   
    }

    [Client]
    public void UpdateTimerTextColor (Color color) { //updates timer text color client side
        timerText.color = color;
    }

    [Client]
    void UpdateBtnText () {

        Debug.Log ("Changing disconnect button text");
        
        switch (NetworkManager.singleton.mode) {
            case NetworkManagerMode.Host:
                foreach (Text _btn in dcBtnText) {
                    _btn.text = "STOP HOSTING";
                }
                break;
            case NetworkManagerMode.ClientOnly:
                foreach (Text _btn in dcBtnText) {
                    _btn.text = "DISCONNECT";
                }
                break;
        }
    }

    [Client]
    public void Disconnect () { //on button press check local player mode and disconnect them or stop hosting entirely
        switch (NetworkManager.singleton.mode) {
            case NetworkManagerMode.Host:
                NetworkManager.singleton.StopHost ();
                break;
            case NetworkManagerMode.ClientOnly:
                NetworkManager.singleton.StopClient ();
                break;
        }
    }

    [Client]
    public void Retry () {
        Game_Manager.instance.CmdReloadOnlineScene ();
    }

}
