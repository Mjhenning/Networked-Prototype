using System;
using LightReflectiveMirror;
using Mirror;
using TMPro;
using UnityEngine;

public class UiConnectionCode : MonoBehaviour {
    [SerializeField] TMP_Text codeText;
    LightReflectiveMirrorTransport lrmTransport;

    void Update () {
        if (NetworkManager.singleton.mode is NetworkManagerMode.Offline or NetworkManagerMode.ClientOnly) return;
        if (!lrmTransport) lrmTransport = NetworkManager.singleton.transport.GetComponent<LightReflectiveMirrorTransport> ();
        if(!lrmTransport) return;
        if (string.IsNullOrEmpty(lrmTransport.serverId)) return;

        codeText.text = lrmTransport.serverId;
    }

    public void CopyToClipboard () {
        if (!lrmTransport) return;
        if (string.IsNullOrEmpty (lrmTransport.serverId)) return;
        GUIUtility.systemCopyBuffer = lrmTransport.serverId;
        Debug.Log ($"Server ID copied to clipboard: {lrmTransport.serverId} ");
    }
}
