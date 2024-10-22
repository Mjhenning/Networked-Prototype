using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

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
        if (UI_Manager.instance)
        {
            startGame.AddListener(UI_Manager.instance.ToggleStartBtn);
        }

        if (TimerManager.instance)
        {
            startGame.AddListener(TimerManager.instance.ToggleTimer);
            endGame.AddListener(TimerManager.instance.ToggleTimer);
        }

        if (ScoreManager.instance)
        {
            startGame.AddListener(ScoreManager.instance.ToggleScoring);
            endGame.AddListener(ScoreManager.instance.ToggleScoring);
        }
    }

    public void ParentToMe(Transform obj)
    {
        obj.parent = transform;
        obj.localScale = Vector3.one;
    }

    // Method for clients to request game end
    [Client]
    public void RequestGameEnd()
    {
        if (isLocalPlayer)
        {
            CmdCallGameEnd();
        }
    }

    // Method for server to trigger game end
    [Server]
    public void CallGameEnd()
    {
        RpcCallGameEnd();
    }

    [Command]
    void CmdCallGameEnd()
    {
        CallGameEnd();
    }

    [ClientRpc]
    void RpcCallGameEnd()
    {
        endGame.Invoke();
        Time.timeScale = 0;
    }

    // Method for clients to request game start
    [Client]
    public void RequestGameStart()
    {
        if (isLocalPlayer)
        {
            CmdCallGameStart();
        }
    }

    // Method for server to trigger game start
    [Server]
    public void CallGameStart()
    {
        RpcCallGameStart();
    }

    [Command]
    void CmdCallGameStart()
    {
        CallGameStart();
    }

    [ClientRpc]
    void RpcCallGameStart()
    {
        startGame.Invoke();
        Debug.Log ("Starting the game");
    }

    [Command(requiresAuthority = false)]
    public void CmdReloadOnlineScene () {
        ReloadOnlineScene ();
    }

    [Server]
    void ReloadOnlineScene () {
        Debug.Log ("Reloading Scene");
        NetworkManager.singleton.ServerChangeScene ("Online");
    }
}