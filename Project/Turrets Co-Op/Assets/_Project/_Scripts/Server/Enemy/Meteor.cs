using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class Meteor : NetworkBehaviour {
    [SerializeField] float meteorSpeed = 3;
    Rigidbody rb;
    [SerializeField] MeshRenderer renderer;
    [SerializeField] Transform effect;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate() {
        if (!isServer) return;
        
        if (transform.position.z < -5) {
            ReturnMe ();
        }
    }

    void OnCollisionEnter (Collision other) {
        if (other.transform.GetComponent<Player>()) {
            Player _player = other.transform.GetComponent<Player> ();
            _player.TweakHealth (false, 1);
            DestroyWEffect ();
        }
        
    }

    [Server]
    public void DestroyWEffect () { //TODO: disable play effect then return
        DisableObj ();
        PlayEffectReturn ();
    }

    [Server]
    void PlayEffectReturn () {
        effect.gameObject.SetActive (true);
        RpcEnableEffect ();
        StartCoroutine (WaitAndDisable ());
    }

    IEnumerator WaitAndDisable () {
        yield return new WaitForSeconds (1.5f);
        effect.gameObject.SetActive (false);
        RpcDisableEffect ();
        ReturnMe ();
    }
    
    [ClientRpc]
    void RpcEnableEffect() {
        effect.gameObject.SetActive (true);
    }
    
    [ClientRpc]
    void RpcDisableEffect() {
        effect.gameObject.SetActive (false);
    }

    [Server]
    void ReturnMe () {
        DisableObj ();
        MeteorPoolManager.Instance.ReturnMeteor (gameObject);
    }

    [Server]
    public void EnableObj() {
        if (!isServer) return;
        
        // Enable the object on the server
        RpcEnableObj();
        
        // Set velocity on the server
        rb.linearVelocity = -(transform.forward * meteorSpeed);
    }

    [ClientRpc]
    void RpcEnableObj() {
        renderer.enabled = true;
        GetComponent<Collider>().enabled = true;
    }

    [Server]
    public void DisableObj() {
        if (!isServer) return;
        
        // Disable the object on the server
        RpcDisableObj();
        
        // Reset velocity on the server
        rb.linearVelocity = Vector3.zero;
    }

    [ClientRpc]
    void RpcDisableObj() {
        renderer.enabled = false;
        GetComponent<Collider>().enabled = false;
    }
}