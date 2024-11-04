using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

public class Meteor : NetworkBehaviour {

    bool splitable;
    [SerializeField] float meteorSpeed = 3;
    Rigidbody rb;
    [SerializeField] MeshRenderer renderer;
    [SerializeField] Transform effect;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable () {
        DecideSplitable (50f);
    }

    void FixedUpdate() {
        if (!isServer) return;
        
        if (transform.position.z < -5) {
            ReturnMe ();
        }
    }

    void OnCollisionEnter (Collision other) { //if collides with player make the player take damage and destroy with ana effect
        if (other.transform.GetComponent<Player>()) {
            Player _player = other.transform.GetComponent<Player> ();
            _player.TakeDamage();
            DestroyWEffect ();
        }
    }

    [Server]
    void DestroyWEffect () { //disables the object and plays an effect
        DisableObj ();
        PlayEffectReturn ();
    }

    [Server]
    public void Split () {

        if (splitable) { //if splittable split me and play effect
            splitable = false;
            Meteor[] _children = new Meteor[3];
            DestroyWEffect ();
        
            _children = MeteorPoolManager.Instance.GetMeteorsForSplit (Random.Range (2, 3)); //grab 2/3 meteors from pool

            foreach (var child in _children) { //foreach child, set child pos to mine, randomise scale, randomize rotate make me usplitable and enable me
                if (child != null) {
                    child.transform.position = transform.position;
                    float _randomUScale = Random.Range (.5f,.95f);
                    child.transform.localScale = new Vector3 (_randomUScale, _randomUScale, _randomUScale);
                    child.transform.Rotate (PickRandomDirection ()); //randomly Rotates Object
                    child.EnableObj ();
                    child.splitable = false;
                }
            }
        } else { //if not splittable destroy me with an effect
            DestroyWEffect ();
        }
        
    }

    [Server]
    Vector3 PickRandomDirection () { //pick a randomized rotation
        float _randomX = Random.Range (-60f, 60f);
        float _randomY = Random.Range (-60f, 60f);

        return new Vector3 (_randomX, _randomY, 0);
    }

    [Server]
    void PlayEffectReturn () { // plays effect and syncs to all clients
        effect.gameObject.SetActive (true);
        RpcEnableEffect ();
        StartCoroutine (WaitAndDisable ());
    }

    IEnumerator WaitAndDisable () { //wait 1.5 seocnds for effect to play then disable effect and return me to pool
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
    void ReturnMe () { //make me splittable again disable me and tell the pool manager to return me
        DecideSplitable (50f);
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
        DecideSplitable (50f);
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
    
    public void DecideSplitable(float percentage)
    {
        if (Random.Range (0f, 100f) < percentage) {
            splitable = true;
        } else {
            splitable = false;
        }
    }
}