using System;
using System.Collections.Generic;
using System.Linq;
using PlayFab.ClientModels;
using UnityEngine;

public class UILeaderboardHigh : MonoBehaviour { //leaderboard manager for ui side
    [SerializeField] PoolUILeaderboardEntry poolUILeaderboardEntry;

    List<UILeaderboardEntry> existingEntries = new List<UILeaderboardEntry> ();

    void OnEnable () {
        UserProfile.OnLeaderBoardHighUpdated.AddListener (LeaderboardScoreUpdated);
    }
    
    void OnDisable () {
        UserProfile.OnLeaderBoardHighUpdated.RemoveListener (LeaderboardScoreUpdated);
    }
    
    void LeaderboardScoreUpdated (List<PlayerLeaderboardEntry> leaderboardEntries) {

        if (existingEntries.Count >0){
            for (int i = existingEntries.Count -1; i >= 0; i--) {
                poolUILeaderboardEntry.ReturnToObjectPool (existingEntries[i]);
            }
        }

        existingEntries.Clear ();
        
        
        for (var i = 0; i < leaderboardEntries.Count; i++) {
            UILeaderboardEntry _entry = poolUILeaderboardEntry.GetFromObjectPool ();
            _entry.SetLeaderboardEntry (leaderboardEntries[i]);
            existingEntries.Add (_entry);
        }
    }
}
