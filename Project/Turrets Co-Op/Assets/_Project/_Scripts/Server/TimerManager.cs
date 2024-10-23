using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class TimerManager : NetworkBehaviour {
    public static TimerManager instance;
    
    [SerializeField] float timeRemaining = 120;

    [SyncVar (hook = nameof (UpdateTimerColor))] [SerializeField]
    Color timerColor;

    [SyncVar (hook = nameof (UpdateTimerText))] [SerializeField]
    string timerText;

    bool timerIsRunning = false;

    bool gameEnded = false;

    void Awake () {
        if (instance == null) instance = this;
    }
    
    [Server] 
    public void ToggleTimer() {
       timerIsRunning = !timerIsRunning;
    }
   
    void Update() {
        if (!isServer) return;

        UpdateTimer ();
    }
    
    [Server]
    void UpdateTimer () {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                // Decrease time remaining
                timeRemaining -= Time.deltaTime;

                UpdateText (timeRemaining);
            }
            else
            {
                // Time is up
                Debug.Log("Time's Up!");
                timeRemaining = 0;
                
                UpdateText (timeRemaining);
                timerIsRunning = false;

                if (!gameEnded) {
                    Game_Manager.instance.CallGameEnd (); //calls endgame event
                    gameEnded = true;
                }
            }
            
            if (timeRemaining > 15) {
                UpdateColor(new Color(1,1,1,.137f)); //set to semi transparent white
            } else if (timeRemaining <= 15) {
                UpdateColor (new Color (1, 0, 0,.137f)); //set to semi transparent red
            }
        }
    }

    [Server]
    void UpdateText (float newTimer) { //updates timer text
        timerText = FormatTime (newTimer);
    }

    [Client]
    void UpdateTimerText (string oldText, string newText) { //reflects client side
        if (!UI_Manager.instance) return;
        UI_Manager.instance.UpdateTimerText (newText);
    }
    
    [Server]
    void UpdateColor (Color newColor) { //updates timer color
        timerColor = newColor;
    }

    [Client]
    void UpdateTimerColor (Color oldColor, Color color) { //reflects client side
        UI_Manager.instance.UpdateTimerTextColor (color);
    }
    
    [Server]
    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f); // Get the total minutes
        int seconds = Mathf.FloorToInt(time % 60f); // Get the remaining seconds
        return $"{minutes}:{seconds:00}";
    }
}
