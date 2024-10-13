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
}