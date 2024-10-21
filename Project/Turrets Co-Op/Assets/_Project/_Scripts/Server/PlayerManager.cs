using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    
    public List<Player> playersList = new List<Player>();

    void Awake()
    {
        instance = this;
    }

    void OnEnable () {
        if (Game_Manager.instance != null) {
            Game_Manager.instance.endGame.AddListener (UpdatePlayerHighs);
        } else {
            StartCoroutine (WaitForGameManager ());
        }
    }
    
    void OnDisable () {
        Game_Manager.instance.endGame.AddListener (UpdatePlayerHighs);
    }
    
    IEnumerator WaitForGameManager() {
        while (Game_Manager.instance == null) {
            yield return null; // Wait for the next frame
        }
    
        // Once Game_Manager.instance is not null, add the listener
        Game_Manager.instance.endGame.AddListener (UpdatePlayerHighs);
    }
    
    [Server] //server side register's player to a list
    public void RegisterPlayer(Player playerRef)
    {
        playersList.Add(playerRef);
    }

    [Server] //serverside de register's a player from a list
    public void DeRegisterPlayer(Player playerRef)
    {
        playersList.Remove(playerRef);

        foreach (Player player in playersList) { //used to update the colors of players on list if a player leaves
            player.SetColor ();
        }
    }
    
    // Method for server to trigger game end Score Update checks for each player
    [Server]
    void UpdatePlayerHighs () {
        foreach (Player _player in playersList) {
            _player.UpdateStats ();
        }
    }
}