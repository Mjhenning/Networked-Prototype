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

    void Awake()
    {
        instance = this;
    }

    void Start() //adds listeners to all online components
    {

        if (ScoreManager.instance != null && isServer) {
            startGame.AddListener(ScoreManager.instance.ToggleScoring);
            endGame.AddListener(ScoreManager.instance.ToggleScoring);
            
        }

        if (TimerManager.instance != null && isServer) {
            startGame.AddListener (TimerManager.instance.ToggleTimer);
            endGame.AddListener (TimerManager.instance.ToggleTimer);
        }

        if (PlayerManager.instance != null && isServer) {
            endGame.AddListener (PlayerManager.instance.UpdatePlayerHighs);

            startGame.AddListener (PlayerManager.instance.TogglePlayerCollisions);
            endGame.AddListener (PlayerManager.instance.TogglePlayerCollisions);
        }
        
    }

    void OnDisable () {
        if (UI_Manager.instance && isClient)
        {
            startGame.RemoveListener(UI_Manager.instance.ToggleStartBtn);
        }

        if (ScoreManager.instance != null && isServer) {
            startGame.RemoveListener(ScoreManager.instance.ToggleScoring);
            endGame.RemoveListener(ScoreManager.instance.ToggleScoring);
            
        }

        if (TimerManager.instance != null && isServer) {
            startGame.RemoveListener (TimerManager.instance.ToggleTimer);
            endGame.RemoveListener (TimerManager.instance.ToggleTimer);
        }

        if (PlayerManager.instance != null && isServer) {
            endGame.RemoveListener (PlayerManager.instance.UpdatePlayerHighs);

            startGame.RemoveListener (PlayerManager.instance.TogglePlayerCollisions);
            endGame.RemoveListener (PlayerManager.instance.TogglePlayerCollisions);
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
            endGame.Invoke();
            
            
            PlayerManager.instance.TogglePlayerCrosshairs();
        
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
    public void CmdReloadOnlineScene () {
        ReloadOnlineScene ();
    }

    [Server]
    public void ReloadOnlineScene () {
        Debug.Log ("Reloading Scene");
        NetworkManager.singleton.ServerChangeScene ("Online");
    }
    
}