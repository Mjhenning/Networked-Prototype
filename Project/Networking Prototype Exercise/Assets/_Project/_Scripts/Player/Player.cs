using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.Mathematics;

public class Player : NetworkBehaviour {

    public Rigidbody rb;
    [SerializeField]float movementSpeed;

    [Header ("Gameplay")]
    [SerializeField] GameObject bulletPref;
    [SerializeField] float fireRate = 0.1f;
    
    [Header("Debug")]
    [SerializeField] bool isFiring = false;
    
    public override void OnStartClient () {
        Debug.Log ($"[Player{netId}] OnStartClient");
        base.OnStartClient ();
    }

    public override void OnStartServer () {
        Debug.Log ($"[Player{netId}] OnStartServer");
        base.OnStartServer ();
    }

    public override void OnStartLocalPlayer () {
        Debug.Log ($"[Player{netId}] OnStartPlayer");
        base.OnStartLocalPlayer ();
        RegisterInputs ();
    }
    
    public override void OnStopLocalPlayer () {
        Debug.Log ($"[Player{netId}] OnStartPlayer");
        base.OnStopLocalPlayer ();
        DeRegisterInputs ();
    }

    void Update () {
        if (isLocalPlayer) HandleMovement ();
    }

    //INPUTS
    
    [Client]
    void DeRegisterInputs () {
        InputHandler.Disable ();
        InputHandler.OnFireStarted -= InputHandlerOnOnFireStarted;
        InputHandler.OnFireCanceled -= InputHandlerOnOnFireCanceled;
    }

    [Client]
    void RegisterInputs () {
        InputHandler.Enable ();
        InputHandler.OnFireStarted += InputHandlerOnOnFireStarted;
        InputHandler.OnFireCanceled += InputHandlerOnOnFireCanceled;
    }

    void InputHandlerOnOnFireStarted (object sender, EventArgs e) {
        HandleFiring (true);
    }

    void InputHandlerOnOnFireCanceled (object sender, EventArgs e) {
        HandleFiring (false);
    }
    
    //GAMEPLAY
    
    [Client]
    void HandleMovement () {
        rb.linearVelocity = new Vector3 (InputHandler.MovementVector.x, 0, InputHandler.MovementVector.y) * (movementSpeed * 0.1f);
    }

    void HandleFiring (bool firing) {
        //TODO: Handle mouse direction aiming
        cmdFireBullet (firing);

    }

    [Command]
    void cmdFireBullet (bool firing) {
        Debug.Log($"[Player][{netId}] cmdFireBullet");
        isFiring = firing;
        if (isFiring) StartCoroutine (FiringBullets());
    }

    [Server]
    IEnumerator FiringBullets () {
        float _cooldown = 0;
        
        while (isFiring) {
            if (_cooldown > 0) {
                _cooldown -= Time.deltaTime;
            } else {
                Debug.Log($"[Player][{netId}] Firing Bullet");
                _cooldown = fireRate;

                SpawnBullet ();
            }
            yield return null;
        }

        void SpawnBullet () {
            GameObject _bullet = Instantiate (bulletPref, transform.position + Vector3.forward * 0.5f, transform.rotation);
            NetworkServer.Spawn (_bullet); //specifically spawns said bullet
        }
    }
}
