using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Org.BouncyCastle.Asn1.Esf;
using UnityEngine;

public class Bullet : NetworkBehaviour {
    [SerializeField] float bulletSpeed = 10;
    [SerializeField] float timeout = 5;

    public override void OnStartServer () {
        StartCoroutine (Timeout ());
    }

    void FixedUpdate() {
        if (!isServer) return;
        transform.Translate (transform.forward * (Time.fixedDeltaTime * bulletSpeed));
    }
    

    [Server]
    IEnumerator Timeout () {
        yield return new WaitForSeconds (timeout);
        DestroySelf ();
    }
    
    [Server]
    void DestroySelf () {
        //TODO: Pool me
        NetworkServer.Destroy (gameObject);
    }

    void OnCollisionEnter (Collision collision) { //Hit handling
    }
}
