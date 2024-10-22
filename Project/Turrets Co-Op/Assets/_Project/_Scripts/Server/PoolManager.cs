using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.Mathematics;

public class PoolManager : NetworkBehaviour
{
    public static PoolManager Instance;
    public GameObject poolPref;
    [ShowInInspector] Queue<GameObject> bulletPool = new Queue<GameObject>();
    public int poolSize = 100;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public override void OnStartServer()
    {
        Game_Manager.instance.ParentToMe(transform);

        // Initialize the bullet pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(poolPref, gameObject.transform);
            NetworkServer.Spawn(bullet);
            bulletPool.Enqueue(bullet);
            bullet.GetComponent<Bullet> ().DisableObj ();
            bullet.transform.position = new Vector3 (0, 100, 0);
        }
    }
    
    [Server]
    public Bullet GetBullet(Color color, Player owner)
    {
        // Dequeue a bullet from the pool or instantiate a new one if the pool is empty
        if (bulletPool.Count > 0)
        {
            GameObject bullet = bulletPool.Dequeue();
            bullet.GetComponent<Bullet> ().SetProperties (color, owner);
            return bullet.GetComponent<Bullet>();
        }
        else
        {
            GameObject bullet = Instantiate(poolPref, gameObject.transform);
            NetworkServer.Spawn(bullet);
            bullet.GetComponent<Bullet> ().DisableObj ();
            return bullet.GetComponent<Bullet>();
        }
    }
    
    
    [Server]
    public void ReturnBullet(GameObject bullet)
    {
        // Reset and deactivate the bullet before enqueuing it back into the pool
        bullet.transform.position = new Vector3 (0, 100, 0);
        bullet.transform.rotation = new quaternion(0, 0, 0, 0);
        bulletPool.Enqueue(bullet);
        bullet.GetComponent<Bullet> ().DisableObj ();
    }
    
}