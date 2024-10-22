using System;
using Mirror;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuNetworkUI : MonoBehaviour
{

    NetworkManagerMode selectedMode = NetworkManagerMode.Host;
    
    [SerializeField]NetworkManager manager;

    [SerializeField] Text btnText;
    

    public void SelectMode (int index) {
        switch (index) {
            case 0:
                selectedMode = NetworkManagerMode.Host; break;
            case 1:
                selectedMode = NetworkManagerMode.ClientOnly; break;
            case 2:
                selectedMode = NetworkManagerMode.ServerOnly; break;
        }
    }

    public void ChangeTextBasedOfMode () {
        switch (selectedMode) {
            case NetworkManagerMode.ClientOnly:
                btnText.text = "JOIN"; break;
            case NetworkManagerMode.Host:
                btnText.text = "HOST"; break;
            case NetworkManagerMode.ServerOnly:
                btnText.text = "START SERVER"; break;
        }
    }

    public void StartSelectedMode () {

        if (UIProfile.instance.playerNameText.text != null &&UIProfile.instance.playerNameText.text != "") {
            switch (selectedMode) {
                case NetworkManagerMode.ClientOnly:
                    manager.StartClient(); break;
                case NetworkManagerMode.Host:
                    manager.StartHost(); break;
                case NetworkManagerMode.ServerOnly:
                    manager.StartServer(); break;
            }
        } else {
            UIProfile.instance.DisplayErrorMessage ();
        }
    }
}
