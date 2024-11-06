using System;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour {
    [SerializeField] float bulletSpeed = 10f;
    public Player playerRef;
    public TrailRenderer bulletTrail;


    Rigidbody rb;

    void Awake () {
        rb = GetComponent<Rigidbody> ();
    }

    void OnCollisionEnter (Collision hit) {
        if (!isServer) return;

        Debug.Log ("Successfully hit");

        if (hit.transform.GetComponent<Meteor> ()) {
            playerRef.AddScore (10, playerRef); // Add score to player
            hit.transform.GetComponent<Meteor> ().Split ();
        }

        BulletPoolManager.Instance.ReturnBullet (gameObject);
    }

    [Server] //enable obj visually server side
    public void EnableObj () {
        if (!isServer) return;

        RpcEnableObj ();

        rb.linearVelocity = transform.forward * bulletSpeed; // Set velocity on the server
    }

    [ClientRpc] //syncs up visual enable client side
    void RpcEnableObj () {
        bulletTrail.SetPositions (Array.Empty<Vector3> ());
        bulletTrail.enabled = true;
        GetComponent<Collider> ().enabled = true;
    }

    [Server] //disable obj visually server side
    public void DisableObj () {
        if (!isServer) return;

        RpcDisableObj ();

        rb.linearVelocity = Vector3.zero; // Reset velocity on the server
    }

    [ClientRpc] //syncs up visual disable client side
    void RpcDisableObj () {
        bulletTrail.SetPositions (Array.Empty<Vector3> ());
        bulletTrail.enabled = false;
        GetComponent<Collider> ().enabled = false;
    }

    [ClientRpc] //syncs bullet transform to clients
    public void RpcUpdateTransform (Vector3 position, Quaternion rotation) {
        transform.position = position;
        transform.rotation = rotation;
    }

    [Server]
    public void SetProperties (Color color, Player owner) {
        //changes bullet properties
        playerRef = owner;
        bulletTrail.startColor = color;
        bulletTrail.endColor = color;
        RpcSetBulletProperties (color, owner);
    }

    [ClientRpc]
    void RpcSetBulletProperties (Color color, Player owner) {
        //syncs bullet changes
        bulletTrail.startColor = color;
        bulletTrail.endColor = color;
    }
}