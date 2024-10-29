using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour { //script holding a list of spawnpoints on both client and host scene
    public static SpawnPointManager Instance;
    
    public List<Transform> spawnPoints; //change to a get function, validation done here nowhere else

    void Awake () {
        Instance = this;
    }

    
    public Transform GetSpawnPoint (int playerIndex) { //called by player to grab the corresponding spawnpoint off of the list of spawnpoints
        if (playerIndex >= 0 && playerIndex < spawnPoints.Count) {
            return spawnPoints[playerIndex];
        } else {
            return null;
        }
    }
}
