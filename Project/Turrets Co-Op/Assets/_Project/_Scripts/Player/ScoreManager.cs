using System;
using System.Collections;
using Mirror;
using UnityEngine;
using TMPro;

public class ScoreManager : NetworkBehaviour //don't change ui only change score
{
    public static ScoreManager instance;

    [SyncVar]public bool gameActive = false;

    
    [SyncVar(hook = nameof(UpdateScoreText))][SerializeField]int score;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    public override void OnStartServer()
    {
        score = 0;
    }
    
    [Server]
    public void ToggleScoring () { //allow/disallow scoring
        gameActive = !gameActive;
    }
    
    [Server] //Updates score server side
    public void UpdateScore(int newScore)
    {
        if (!gameActive) return;
        score += newScore;
    }
    
    [Client]
    void UpdateScoreText(int oldScore,int newScore) //updates text client side
    {
        if (UI_Manager.instance)
        {
            UI_Manager.instance.UpdateScoreText(newScore);
        }
    }

    [Server]
    public void ResetGameScore () {
        score = 0;
        gameActive = false;
    }
    
    
    
    
}