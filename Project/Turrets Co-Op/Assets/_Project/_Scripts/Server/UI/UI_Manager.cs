using System;
using Mirror;
using TMPro;
using UnityEngine;

public class UI_Manager : MonoBehaviour { 
    
    public static UI_Manager instance;
    
    [SerializeField] GameObject startBtn;
    [SerializeField] TMP_Text scoreText; // Reference to the UI Text component
    [SerializeField] TMP_Text timerText;

    [SerializeField] GameObject endScreen;
    [SerializeField] TMP_Text endScoreTxt;
    [SerializeField] TMP_Text personalEndScoreTxt;

    void Awake () {
        instance = this;
    }
    
    void Start () {

        if (Game_Manager.instance) {
            Game_Manager.instance.endGame.AddListener (ToggleGameOver);
            
        }

        ToggleButtonVisibility ();
    }

    public void ToggleButtonVisibility () {
        if (NetworkManager.singleton.mode == NetworkManagerMode.Host){ //TODO: need a way that the host (first client that joins active server) can start instead of only the server
            startBtn.SetActive (!startBtn.activeSelf);  
        }
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
 
}
