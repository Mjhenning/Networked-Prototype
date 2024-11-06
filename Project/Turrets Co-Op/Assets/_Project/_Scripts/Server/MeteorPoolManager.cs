using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MeteorPoolManager : NetworkBehaviour {
    [Header ("Pool Setup")]
    public static MeteorPoolManager Instance;

    public GameObject meteorPrefab;
    [ShowInInspector] Queue<GameObject> meteorPool = new Queue<GameObject> ();
    public int poolSize = 40;

    [Header ("Wall Setup")]
    // Define the plane (wall) dimensions
    public Vector2 wallSize = new Vector2 (10f, 5f);

    public Transform wallCenter;

    [Header ("Generation Setup")]
    // Spawning parameters
    public float minSpawnInterval = 1f;

    public float maxSpawnInterval = 5f;

    void Awake () {
        if (Instance == null) Instance = this;
    }


    public override void OnStartServer () {
        // Initialize the meteor pool
        for (int i = 0; i < poolSize; i++) {
            GameObject meteor = Instantiate (meteorPrefab, gameObject.transform);
            meteor.GetComponent<Meteor> ().DisableObj ();
            meteor.transform.position = new Vector3 (0, 50, 0);
            NetworkServer.Spawn (meteor);
            meteorPool.Enqueue (meteor);
        }

        // Start the random spawning coroutine
        StartCoroutine (SpawnMeteorsRandomly ());
    }

    [Server]
    void GetMeteor () {
        // Dequeue a meteor from the pool or instantiate a new one if the pool is empty
        if (meteorPool.Count > 0) {
            GameObject meteor = meteorPool.Dequeue ();
            meteor.transform.position = new Vector3 (0, 50, 0);
            meteor.GetComponent<Meteor> ().EnableObj ();
            meteor.transform.position = GetRandomPositionOnWall ();
        } else {
            GameObject meteor = Instantiate (meteorPrefab, gameObject.transform);
            meteor.transform.position = new Vector3 (0, 50, 0);
            NetworkServer.Spawn (meteor);
            meteor.transform.position = GetRandomPositionOnWall ();
        }
    }

    [Server]
    public Meteor[] GetMeteorsForSplit (int childAmount) {
        Meteor[] children = new Meteor[3];

        if (meteorPool.Count > 0) {
            for (int i = 0; i < childAmount; i++) {
                GameObject meteor = meteorPool.Dequeue ();
                meteor.transform.position = new Vector3 (0, 50, 0);
                children[i] = meteor.GetComponent<Meteor> ();
            }

            return children;
        } else {
            for (int i = 0; i < childAmount; i++) {
                GameObject meteor = Instantiate (meteorPrefab, gameObject.transform);
                meteor.transform.position = new Vector3 (0, 50, 0);
                NetworkServer.Spawn (meteor);
                children[i] = meteor.GetComponent<Meteor> ();
            }

            return children;
        }

        return null;
    }

    [Server]
    public void ReturnMeteor (GameObject meteor) {
        // Reset and deactivate the meteor before enqueuing it back into the pool
        meteor.transform.localScale = Vector3.one;
        meteor.transform.rotation = new Quaternion (0, 0, 0, 0);
        meteor.transform.position = new Vector3 (0, 50, 0);
        meteorPool.Enqueue (meteor);
    }

    [Server]
    Vector3 GetRandomPositionOnWall () //grabs a random position on a invisible wall 70 units away from camera
    {
        // Calculate a random position within the bounds of the wall
        float randomX = UnityEngine.Random.Range (-wallSize.x / 2, wallSize.x / 2);
        float randomY = UnityEngine.Random.Range (-wallSize.y / 2, wallSize.y / 2);
        Vector3 randomPosition = new Vector3 (randomX, randomY, 0f) + wallCenter.position;
        return randomPosition;
    }

    [Server]
    IEnumerator SpawnMeteorsRandomly () //spawns at random intervats
    {
        while (true) {
            float spawnInterval = UnityEngine.Random.Range (minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds (spawnInterval);

            GetMeteor ();
        }
    }
}