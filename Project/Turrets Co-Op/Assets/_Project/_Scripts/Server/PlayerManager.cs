using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager instance;

    public List<Player> playersList = new List<Player>();
    

    void Awake()
    {
        instance = this;
    }
    

    [Server]
    public void RegisterPlayer(Player playerRef) {
        playersList.Add(playerRef);

        // Check if this is the first player to join
        if (playersList.IndexOf(playerRef) == 0 && playerRef.connectionToClient != null) {
            // Send the TargetRpc to the first player's connection
            TogglePlayerBtnsNText(playerRef.connectionToClient, playerRef);
        }
    }
    
    [Server]
    public void DeRegisterPlayer(Player playerRef) {
        // Remove player's score on disconnect
        playerRef.RemoveScoreOnDisconnect();

        // Get the index of the player who is disconnecting
        int playerIndex = playersList.IndexOf(playerRef);

        // Remove the player from the list
        playersList.Remove(playerRef);

        // Sync the updated player list across clients
        SyncPlayers();

        // If the disconnecting player is the host (index 0)
        if (playerIndex == 0) {
            // If there are still players left, assign the next player as host
            if (playersList.Count > 0) {
                Player newHost = playersList[0]; // The new host is the first player in the list
                if (newHost.connectionToClient != null && !ScoreManager.instance.gameActive) {
                    // Update client buttons for the new host and assign host controls
                    HostGameStartChanges(newHost.connectionToClient, newHost);
                } else if (newHost.connectionToClient != null) {
                    UpdateClientBtns (newHost.connectionToClient, newHost);
                }
            }
        } else {
            // If the disconnecting player is not the host, no host reassignment is needed
            // Just update client buttons or other relevant UI for the new player
            Player newHost = playersList.Count > playerIndex ? playersList[playerIndex] : null;
            if (newHost != null && newHost.connectionToClient != null) {
                UpdateClientBtns(newHost.connectionToClient, newHost);
            }
        }

        CheckIfSceneShouldRestart ();
    }

    
    
    //TOGGLES BASED ON GAME END / START FOR UI ELEMENTS

    [Server]
    void TogglePlayerBtnsNText(NetworkConnection target, Player _player) {
        StartCoroutine(WaitForUIManager(_player));
    }
    
    [Server]
    IEnumerator WaitForUIManager(Player _player) //waits for ui_manager then toggles host start and retry buttons
    {
        while (UI_Manager.instance == null)
        {
            yield return null; // Wait for the next frame
        }
        
        HostGameStartChanges(_player.connectionToClient,_player);
    }

    [Server]
    void HostGameStartChanges (NetworkConnection target, Player _player) { //Calls ui changes for the host(first) player
        ToggleClientStart (target, _player);
        UpdateClientBtns (target,_player);
    }

    [TargetRpc]
    void ToggleClientStart (NetworkConnection target, Player _playerRef) { //toggles start btn visibility
        UI_Manager.instance.ToggleButtonVisibility(_playerRef);
    }
    

    [TargetRpc]
    void UpdateClientBtns(NetworkConnection target, Player _playerRef) { //toggles text of disconnect buttons
        UI_Manager.instance.UpdateBtnText(_playerRef);
    }
    
    [Server]
    public void TogglePlayerCrosshairs () { //toggles player crosshair on or off
        if (isServer) {
            foreach (Player _player in playersList) {
                _player.ToggleCrosshair ();
            }
        }
    }
    
    
    // Method for server to trigger game end Score Update checks for each player
    [Server]
    public void UpdatePlayerHighs () {
        if (isServer) {
            foreach (Player _player in playersList) {
                _player.UpdateStats ();
            }
        }
    }
    
    
    //DEATh TRIGGERS

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
    void SyncPlayers () { //synchronizes player color and position after another player leaves
        foreach (Player _player in playersList) {
            _player.SyncMe ();
        }
    }

    [Server]
    void CheckIfSceneShouldRestart () { //used to check if server is empty
        if (playersList.Count == 0) {
            StartCoroutine (WaitThenRestart());
        }
    }

    IEnumerator WaitThenRestart () { //if server is empty wait 2 seconds and restart the scene
        yield return new WaitForSeconds (2f);
        Game_Manager.instance.ResetOnlineScene  ();
    }
    
    //WINNER TXT LOGIC

    [Server]
    public void CheckWhoWon() {
        if (playersList.Count == 0) return;

        // Assume the first player has the highest score initially
        Player winningPlayer = playersList[0];

        // Iterate over players to find the highest score
        foreach (Player _player in playersList) {
            if (_player.GetScore() > winningPlayer.GetScore()) {
                winningPlayer = _player;
            }
        }

        // Call Rpc to set label with the winning player's color
        RpcSetLbl(winningPlayer.GetColor());
    }
    
    //winner text

    [ClientRpc]
    void RpcSetLbl (Color winnerColor) {
        UI_Manager.instance.SetWinnerLbl(winnerColor);
    }
    
    //Scene restart logic
    [Server]
    public void ResetPlayers () {
        if (playersList.Count > 0) { //if list has players
            foreach (Player _player in playersList) {
                _player.ResetMe ();
            }

            TogglePlayerCrosshairs (); //sets crosshairs active again and allows input for them

            HostGameStartChanges (playersList[0].connectionToClient, playersList[0]); //by default player at top of list is player 1 
        }
       
    }
}