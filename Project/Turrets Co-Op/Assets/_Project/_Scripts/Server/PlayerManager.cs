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
    public void UpdatePlayerHighs () {
        foreach (Player _player in playersList) {
            _player.UpdateStats ();
        }
    }

    [Server]
    public void TogglePlayerCollisions () {
        if (isServer) {
            foreach (Player _player in playersList) {
                if (_player != null) {
                    _player.ChangeCollision ();
                }
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