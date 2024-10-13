using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour {

    bool outGame = true;
    
    [SerializeField] GameObject copyCodeObj;
    [SerializeField] NetworkManagerHUD connectHud;
    
    void Start () {
        if (Game_Manager.instance) {
            Game_Manager.instance.startGame.AddListener (ToggleNetworkUI);
            Game_Manager.instance.endGame.AddListener (ToggleNetworkUI);
        } else {
            StartCoroutine(WaitForGameManager());
        }
    }

    IEnumerator WaitForGameManager() {
        while (Game_Manager.instance == null) {
            yield return null; // Wait for the next frame
        }
    
        // Once Game_Manager.instance is not null, add the listeners
        Game_Manager.instance.startGame.AddListener (ToggleNetworkUI);
        Game_Manager.instance.endGame.AddListener (ToggleNetworkUI);
    }

    [Client]
    void ToggleNetworkUI () { //used to enable /disable connect code and client connect / discconnect network hud

        Debug.Log ("Toggling UI");
        
        outGame = !outGame; //toggle bool
        
        copyCodeObj.SetActive (outGame);
        connectHud.enabled = outGame;
    }

    public void Quit () { //forcefully quits game
        Debug.Log ("Quitting Game");
        Application.Quit ();
    }
    
}
