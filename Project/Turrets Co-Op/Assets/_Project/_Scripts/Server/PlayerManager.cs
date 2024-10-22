using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager instance;

    public List<Player> playersList = new List<Player>();

    public UnityEvent listChanged = new UnityEvent ();

    void Awake()
    {
        instance = this;
    }

    void OnEnable () {
        if (Game_Manager.instance != null) {
            Game_Manager.instance.endGame.AddListener (UpdatePlayerHighs);
            Game_Manager.instance.endGame.AddListener (TogglePlayerCollisions);
            Game_Manager.instance.startGame.AddListener (TogglePlayerCollisions);

            listChanged.AddListener (SyncPlayers);
        } else {
            StartCoroutine (WaitForGameManager ());
        }
    }
    
    void OnDisable () {
        Game_Manager.instance.endGame.RemoveListener (UpdatePlayerHighs);
        Game_Manager.instance.endGame.RemoveListener (TogglePlayerCollisions);
        
        Game_Manager.instance.startGame.RemoveListener (TogglePlayerCollisions);
    }
    
    IEnumerator WaitForGameManager() {
        while (Game_Manager.instance == null) {
            yield return null; // Wait for the next frame
        }
    
        // Once Game_Manager.instance is not null, add the listener
        Game_Manager.instance.endGame.AddListener (UpdatePlayerHighs);
    }
    
    [Server] 
    public void RegisterPlayer(Player playerRef) {
        playersList.Add(playerRef);

        listChanged.Invoke ();
        
        // Check if this is the first player to join
        if (playersList.Count == 1)
        {
            // Call TargetRpc to toggle the buttons for the first player
            TogglePlayerBtns(playerRef.connectionToClient); // Send the TargetRpc to the first player's connection
        }
    }
    
    [Server] //serverside de register's a player from a list
    public void DeRegisterPlayer(Player playerRef) {
        playerRef.RemoveScoreOnDisconnect ();
        playersList.Remove(playerRef);
        listChanged.Invoke ();
    }

    [TargetRpc]
    void TogglePlayerBtns (NetworkConnection target) {
        StartCoroutine(WaitForUIManager());
    }
    
    IEnumerator WaitForUIManager() //waits for ui_manager then toggles host start and retry buttons
    {
        while (UI_Manager.instance == null)
        {
            yield return null; // Wait for the next frame
        }

        UI_Manager.instance.ToggleButtonVisibility();
    }
    
    
    // Method for server to trigger game end Score Update checks for each player
    [Server]
    void UpdatePlayerHighs () {
        foreach (Player _player in playersList) {
            _player.UpdateStats ();
        }
    }

    [Server]
    void TogglePlayerCollisions () {
        foreach (Player _player in playersList) {
            if (_player != null) {
                _player.CallChangeCollision ();
            }
        }
    }

    [Server]
    void SyncPlayers () {
        foreach (Player _player in playersList) {
            _player.SyncMe ();
        }
    }
    
}