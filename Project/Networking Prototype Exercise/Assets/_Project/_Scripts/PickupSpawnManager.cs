using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PickupSpawnManager : MonoBehaviour {
    public static PickupSpawnManager Instance;

    [SerializeField] List<PickupConfig> pickupConfigs = new List<PickupConfig> ();

    void Awake () {
        if (NetworkManager.singleton.mode == NetworkManagerMode.ClientOnly) DestroyImmediate (gameObject);
            Instance = this;
    }

    [System.Serializable]
    class PickupConfig {
        public GameObject pickupPrefab;
        public int amount = 1;
        public float spawnRadius;
    }
}
