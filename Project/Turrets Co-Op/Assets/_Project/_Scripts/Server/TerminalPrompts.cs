using System;
using System.Collections.Generic;
using System.Threading;
using Mirror;
using UnityEngine;

public class TerminalPrompts : MonoBehaviour {
    NetworkManager manager;
    bool applicationRunning = false;
    bool serverRunning = false;

    [SerializeField] List<GameObject> nonServerObjs = new List<GameObject> ();

    void Start () {
#if UNITY_STANDALONE_LINUX && UNITY_SERVER
        Debug.Log ("Linux server detected - initializing server-only functionality.");
        manager = GetComponent<NetworkManager> ();
        DestroyNonServerObjects ();
        StartServerCommands ();
#elif UNITY_STANDALONE_WIN
        Destroy(this);
#endif
    }

    void StartServerCommands () {
        if (!NetworkServer.active) {
            applicationRunning = true;
            Debug.Log ("Here's a list of usable commands: \n start, used to start the server \n stop, used to stop the server \n exit, used to exit the application");

            // Start a new thread for listening to commands
            Thread commandThread = new Thread (ListenForCommands);
            commandThread.Start ();
        }
    }

    void ListenForCommands () {
        while (applicationRunning) {
            string input = Console.ReadLine ();

            if (!string.IsNullOrEmpty (input)) {
                switch (input.Trim ().ToLower ()) {
                    case "start":
                        StartServer ();
                        break;
                    case "stop":
                        StopServer ();
                        break;
                    default:
                        Debug.Log ("Unknown command. Please use: start or stop");
                        break;
                }
            }
        }
    }

    void StartServer () {
        if (!serverRunning) {
            Debug.Log ("Starting the server...");
            manager.StartServer ();
            serverRunning = true;
        } else {
            Debug.Log ("Server is already running.");
        }
    }

    void StopServer () {
        if (serverRunning) {
            Debug.Log ("Stopping the server...");
            manager.StopServer ();
            serverRunning = false;
        } else {
            Debug.Log ("Server is not currently running.");
        }
    }

    void DestroyNonServerObjects () { //used to remove playfab stuff and their ui on the server
        foreach (GameObject _obj in nonServerObjs) {
            nonServerObjs.Remove (_obj);
            Destroy (_obj);
        }
    }
}