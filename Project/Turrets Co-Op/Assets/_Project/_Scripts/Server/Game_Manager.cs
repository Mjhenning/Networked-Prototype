using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

//Online scene overall manager


public class Game_Manager : NetworkBehaviour
{
    public static Game_Manager instance;

    public UnityEvent endGame = new UnityEvent();
    public UnityEvent startGame = new UnityEvent();
    public UnityEvent resetGame = new UnityEvent ();

    void Awake()
    {
        instance = this;
    }

    void Start() //adds listeners to all online components
    {

        if (ScoreManager.instance != null && isServer) {
            startGame.AddListener(ScoreManager.instance.ToggleScoring);
            
            endGame.AddListener(ScoreManager.instance.ToggleScoring);
            resetGame.AddListener (ScoreManager.instance.ResetGameScore);
        }

        if (TimerManager.instance != null && isServer) {
            startGame.AddListener (TimerManager.instance.ToggleTimer);
            
            endGame.AddListener (TimerManager.instance.ToggleTimer);
            resetGame.AddListener (TimerManager.instance.ResetTimer);
        }

        if (PlayerManager.instance != null && isServer) {
            endGame.AddListener (PlayerManager.instance.UpdatePlayerHighs);

            startGame.AddListener (PlayerManager.instance.TogglePlayerCollisions);
            endGame.AddListener (PlayerManager.instance.TogglePlayerCollisions);

            resetGame.AddListener (PlayerManager.instance.ResetPlayers);
        }
        
    }

    void OnDisable () {
        if (UI_Manager.instance && isClient)
        {
            startGame.RemoveListener(UI_Manager.instance.ToggleStartBtn);
        }

        if (ScoreManager.instance != null && isServer) {
            startGame.RemoveListener(ScoreManager.instance.ToggleScoring);
            resetGame.RemoveListener(ScoreManager.instance.ToggleScoring);
            
            resetGame.RemoveListener (ScoreManager.instance.ResetGameScore);
        }

        if (TimerManager.instance != null && isServer) {
            startGame.RemoveListener (TimerManager.instance.ToggleTimer);
            resetGame.RemoveListener (TimerManager.instance.ToggleTimer);
            
            resetGame.RemoveListener (TimerManager.instance.ResetTimer);
        }

        if (PlayerManager.instance != null && isServer) {
            endGame.RemoveListener (PlayerManager.instance.UpdatePlayerHighs);

            startGame.RemoveListener (PlayerManager.instance.TogglePlayerCollisions);
            endGame.RemoveListener (PlayerManager.instance.TogglePlayerCollisions);
            
            resetGame.RemoveListener (PlayerManager.instance.ResetPlayers);
        }
    }

    public void ParentToMe(Transform obj)
    {
        obj.parent = transform;
        obj.localScale = Vector3.one;
    }
    
    
    //CALL GAME END

    // Method for server to trigger game end
    [Server]
    public void CallGameEnd()
    {
        if (isServer) {
            
            PlayerManager.instance.TogglePlayerCrosshairs();
            
            endGame.Invoke();

           
            PlayerManager.instance.CheckWhoWon ();
        
            Debug.Log ("Ending the game Serverside");
        }

        RpcCallGameEnd ();
    }
    
    [ClientRpc]
    void RpcCallGameEnd()
    {
        endGame.Invoke();
        Time.timeScale = 0;
        
        Debug.Log ("Ending the game Clientside");
    }
    
    //CALL GAME START    SETUP IN THIS WAY BECAUSE START GAME IS ONLY CALLED FROM CLIENT
    
    // Method for server to trigger game start
    [Server]
    void CallGameStart()
    {
        startGame.Invoke();
        Debug.Log ("Starting the game Serverside");
    }

    [Command(requiresAuthority = false)]
    void CmdCallGameStart() { //command used to fire start game on both clients and server
        RpcCallGameStart ();

        if (isServer) { CallGameStart ();} //only if isServer call the game start for the server too
    }

    [ClientRpc]
    void RpcCallGameStart() //syncs start game to all cleints
    {
        startGame.Invoke();
        Debug.Log ("Starting the game Clientside");
    }
    

    //CALL GAME RELOAD
    
    [Command(requiresAuthority = false)]
    public void CmdResetOnlineScene () {
        ResetOnlineScene ();
    }

    [Server]
    public void ResetOnlineScene () { //instead of reloading scene, turn off gameover screen, reset scores.
        
        if (!isServer) return;

        //end game resets timer and scoring toggle for each player
        if (!(ScoreManager.instance.gameActive || TimerManager.instance.gameEnded)) return;
        Debug.Log ("Resetting Online Scene");
        resetGame.Invoke ();
            
        RpcCallGameReset();
    }

    [ClientRpc]
    void RpcCallGameReset() {
        resetGame.Invoke ();
        Time.timeScale = 1;
    }
    
}