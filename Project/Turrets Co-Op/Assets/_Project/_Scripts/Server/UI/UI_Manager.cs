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
    [SerializeField] GameObject winnerTxt;

    [SerializeField] List<Text> dcBtnText = new List<Text> ();
    [SerializeField] Button DCBtnInGame;

    void Awake () {
        instance = this;
    }
    
    [Client]
    void Start() {
        Game_Manager.instance.endGame.AddListener(ToggleGameOver);
        Game_Manager.instance.resetGame.AddListener (ToggleGameOver);
    }

    [Client]
    public void ToggleBtnsInteractivity () {
        
        DCBtnInGame.interactable = !DCBtnInGame.interactable;
        hostStartBtn.GetComponent<Button> ().interactable = !hostStartBtn.GetComponent<Button> ().interactable;
    }


    [Client]
    public void ToggleButtonVisibility (Player playerRef) { //calls togglestartBtn if current player is the first player on the server

        Debug.Log ("Toggling button visibility " + "Current player count is " + PlayerManager.instance.playersList.Count);
        
        if (PlayerManager.instance.playersList.IndexOf(playerRef) > 0 || PlayerManager.instance.playersList.Count > 1) return;
        
        ToggleStartBtn ();
    }

    [Client]
    public void ToggleStartBtn () { //toggles start btn visibility
        Debug.Log ("Toggling Start Btn");
        
        hostStartBtn.SetActive (!hostStartBtn.activeSelf);
        hostRetryBtn.SetActive (!hostRetryBtn.activeSelf);
    }



    [Client]
    public void UpdateBtnText (Player _playerRef) {  //Changes both disconnect btns' text depending on if this is the first player or not
        
        Debug.Log ("Changing disconnect button text " + "Current player index " + PlayerManager.instance.playersList.IndexOf(_playerRef));

        if (PlayerManager.instance.playersList.IndexOf(_playerRef) == 0 || PlayerManager.instance.playersList.Count <= 1) {
            foreach (Text _btn in dcBtnText) {
                _btn.text = "STOP HOSTING";
            }
        } else {
            foreach (Text _btn in dcBtnText) {
                _btn.text = "DISCONNECT";
            } 
        }
    }
    
    
    [Client]
    void ToggleGameOver () { //enables game over UI and disable client crosshairs
        endScreen.SetActive (!endScreen.activeSelf);
        scoreText.gameObject.SetActive (!scoreText.gameObject.activeSelf);
    }
    
    [Client]
    public void UpdatePersonalScoreText (int amount) { //Updates game over ui personal score text 
        if (personalEndScoreTxt) {
            personalEndScoreTxt.text = "Personal: " + Environment.NewLine + amount;
        }
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
    public void SetWinnerLbl (Color winnerColor) {
        winnerTxt.SetActive (true);
        winnerTxt.GetComponent<TMP_Text> ().color = winnerColor;
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
    public void Disconnect () { //on button press check local player mode and disconnect them or stop hosting entirely
        switch (NetworkManager.singleton.mode) {
            case NetworkManagerMode.Host:
                NetworkManager.singleton.StopHost ();
                break;
            case NetworkManagerMode.ClientOnly: //deregister player inputs then dc player (NOT THE BEST APPROACH BUT IT WORKS)
                Player _refPlayer = FindFirstObjectByType (typeof (Player)) as Player;
                _refPlayer.DeRegisterInputs ();
                NetworkManager.singleton.StopClient ();
                break;
        }
    }

    [Client]
    public void Retry () { //calls the reload scene command on the serverside game manager when retry btn is pressed
        Game_Manager.instance.CmdResetOnlineScene ();
    }

}
