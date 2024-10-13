using System;
using Mirror;
using UnityEngine;
using System.Threading;
using LightReflectiveMirror;

public class TerminalPrompts : MonoBehaviour {

    NetworkManager manager; 
    Thread commandThread;
    bool serverRunning = false;
    bool applicationRunning = false;

    void Start()
    {
    #if UNITY_STANDALONE_LINUX && UNITY_SERVER
        Debug.Log("Linux server detected - initializing server-only functionality.");
        StartSeverCommands();
    #elif UNITY_STANDALONE_WIN
        Destroy(this);
    #endif
    }

     void StartSeverCommands () {
         if (!NetworkServer.active) {
             applicationRunning = true;
             commandThread = new Thread (ListenForCommands);
             commandThread.Start ();
             
             Debug.Log ("Here's a list of usable commands: \n start, used to start the server \n stop, used to stop the server \n exit, used to exit the linux application");
         } 
     }

     void Update () {
    #if UNITY_STANDALONE_LINUX && UNITY_SERVER
         if (NetworkServer.active) {
             serverRunning = true;
         }
    #endif
     }

     void ListenForCommands () {
         
         while (applicationRunning) {
             string input = Console.ReadLine ();

             if(Input.GetKeyDown("enter")){
                 switch (input) {
                    case "stop":
                        StopServer ();
                        break;
                    case "exit":
                        ExitApp ();
                        break;
                } 
             }
         }
         
         
         while (serverRunning) {
             string input = Console.ReadLine ();

             if (Input.GetKeyDown ("enter")) {
                 if (input == "start") {
                     Debug.Log ("Command received to start the server.");
                     manager.StartServer ();
                 }
             }
         }
         
     }

     void StopServer () {
         Debug.Log ("Command received to stop the server.");
         serverRunning = false;
         manager.StopServer ();
     }

     void ExitApp () {

         Debug.Log ("Closing application.");
         
         applicationRunning = false;
         
         if (commandThread != null && commandThread.IsAlive)
         {
             commandThread.Join();  // Wait for the command thread to finish
             Debug.Log("Command thread has been safely stopped.");
         }

         Application.Quit ();
     }

   
}
