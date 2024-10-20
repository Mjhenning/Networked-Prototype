using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILeaderboardEntry : MonoBehaviour { //Specific entry in leaderboard script

    [SerializeField] TextMeshProUGUI leaderboardNameText;
    [SerializeField] TextMeshProUGUI leaderboardScoreText;

    public void SetLeaderboardEntry (PlayFab.ClientModels.PlayerLeaderboardEntry entry) {
        leaderboardNameText.text = $"{entry.Position}. {entry.DisplayName}";
        leaderboardScoreText.text = entry.StatValue.ToString ();
    }

}