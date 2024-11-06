using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class TimerManager : NetworkBehaviour {
    public static TimerManager instance;
    
    [SerializeField] float timeRemaining = 120;

    [SyncVar][SerializeField] Color timerColor = new Color (1, 1, 1, .137f);

    [SyncVar (hook = nameof (UpdateTimerText))] [SerializeField]
    string timerText;

    [SyncVar][SerializeField]bool timerIsRunning = false;

    [SyncVar]public bool gameEnded = false;

    void Awake () {
        if (instance == null) instance = this;
    }

    void Start () {
        if (!ScoreManager.instance) return;
        if (!ScoreManager.instance.gameActive) return;
        if(isClient){SetClientUIOnJoin ();}
    }

    [Server] 
    public void ToggleTimer() { //toggles timer boolean
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
                timerColor = new Color (1, 1, 1, .137f);
                UpdateTimerColor(); //set to semi transparent white
            } else if (timeRemaining <= 15) {
                timerColor = new Color (1, 0, 0, .137f);
                UpdateTimerColor (); //set to semi transparent red
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

    [Client]
    void SetClientUIOnJoin () { //sets client text and color on join
        UI_Manager.instance.UpdateTimerTextColor (new Color(1,1,1,.137f));
        UI_Manager.instance.UpdateTimerText (FormatTime (120f));
        Debug.Log ("Set timer color and text when client joined");
    }
    
    [Server]
    void UpdateTimerColor () { //updates timer color
        RpcUpdateTimerColor ();
    }

    [ClientRpc]
    void RpcUpdateTimerColor () { //reflects client side
        UI_Manager.instance.UpdateTimerTextColor (timerColor);
    }
    
    
    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f); // Get the total minutes
        int seconds = Mathf.FloorToInt(time % 60f); // Get the remaining seconds
        return $"{minutes}:{seconds:00}";
    }

    [Server]
    public void ResetTimer () {

        timerIsRunning = false;
        gameEnded = false;
        
        timeRemaining = 120f;
        UpdateText (120f);
        timerColor = new Color (1, 1, 1, .137f);
        StartCoroutine (WaitAFrameAnfChangeColor());
        
    }

    IEnumerator WaitAFrameAnfChangeColor () {
        yield return null;
        UpdateTimerColor(); //set to semi transparent white
    }
}
