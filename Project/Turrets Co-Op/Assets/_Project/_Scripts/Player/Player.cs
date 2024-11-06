using System;
using System.Collections;
using UnityEngine;
using Mirror;
using Cursor = UnityEngine.Cursor;
using UnityEngine.UI;


//Script summary: Player manager script itself. Specifically moves crosshair and syncs it's color. Specifically rotates turret. Also handle calling bullets and firing them.

public class Player : NetworkBehaviour {
    [Header ("Setup")]
    [SerializeField] LayerMask playerMask;
    [SerializeField] bool isFiring = false;
    [SerializeField] MeshRenderer playerRenderer;
    [SerializeField] Collider playerCol;

    [SyncVar (hook = nameof (OnColorChanged))] [ShowInInspector] Color playerColor;

    [Header ("Gameplay")]
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] float lastFireTime;
    [SyncVar][SerializeField] bool isScoring = false;

    [SyncVar(hook = nameof(OnHealthChanged))][ShowInInspector] int currentHealth = 3;
    readonly int maxHealth = 3;

    [Header ("UI")]
    [SerializeField] Crosshair chInstance;
    [SerializeField] PlayerScore scoreInstance;
    [SyncVar (hook = nameof (OnScoreChanged))] [ShowInInspector] int playerScore;
    Image chImg;
    [SerializeField]bool nonCursor = false;
    
    public override void OnStartClient () {
        Debug.Log ($"[Player{netId}] OnStartClient");
        Time.timeScale = 1;
    }

    public override void OnStopClient () {
        Debug.Log ($"[Player{netId}] OnStopClient");
       
    }

    public override void OnStartServer () {
        Debug.Log ($"[Player{netId}] OnStartServer");

        RegisterMe ();
        StartCoroutine (InitializePlayer ());
    }

    public override void OnStopServer () {
        Debug.Log ($"[Player{netId}] OnStopServer");

       DeRegisterMe ();
    }

    public override void OnStartLocalPlayer () {
        Debug.Log ($"[Player{netId}] OnStartLocalPlayer");

        RegisterInputs ();

        if (ScoreManager.instance.gameActive) { //for late users so they have collisions
            CmdChangeCollision ();
            CmdToggleScoring ();
        }
    }

    public override void OnStopLocalPlayer () {
        Debug.Log ($"[Player{netId}] OnStopLocalPlayer");

        DeRegisterInputs ();
    }
    
    
    void OnDestroy()
    {
        StopAllCoroutines(); // Stop all running coroutines when the player is destroyed
    }
    
    void Update () { //handle look and cooldowns
        if (isLocalPlayer) HandleLook ();
        if (isLocalPlayer && GetCooldownProgress () <= 1f) ChangeFillAmount(GetCooldownProgress());
    }
    
    void Start () {
        if (Game_Manager.instance != null && isServer) {
            Game_Manager.instance.startGame.AddListener (ToggleScoring);
            Game_Manager.instance.endGame.AddListener (ToggleScoring);
        }
    }
    
    
    //Registration and Initialization

    [Server]
    void RegisterMe () {
        PlayerManager.instance.RegisterPlayer (this);
    }

    [Server]
    void DeRegisterMe () {
        PlayerManager.instance.DeRegisterPlayer (this);
    }

    [Server]
    IEnumerator InitializePlayer () {
        yield return new WaitForSeconds (1f);
        SetPlaySpawnPos ();
        SetColor ();
    }
    

    [Client]
    public void DeRegisterInputs () {
        InputHandler.Disable ();
        InputHandler.OnFireStarted -= InputHandlerOnOnFireStarted;
        InputHandler.OnFireCanceled -= InputHandlerOnOnFireCanceled;
        InputHandler.OnSwapPerformed -= InputHandlerOnOnSwapPerformed;
        InputHandler.Dispose ();
    }

    [Client]
    void RegisterInputs () {
        InputHandler.Enable ();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        InputHandler.OnFireStarted += InputHandlerOnOnFireStarted;
        InputHandler.OnFireCanceled += InputHandlerOnOnFireCanceled;
        InputHandler.OnSwapPerformed += InputHandlerOnOnSwapPerformed;
    }
    void InputHandlerOnOnSwapPerformed (object sender, EventArgs e) {
        SwapMode (!nonCursor);
    }

    void InputHandlerOnOnFireStarted (object sender, EventArgs e) {
        HandleFiring (true);
    }

    void InputHandlerOnOnFireCanceled (object sender, EventArgs e) {
        HandleFiring (false);
    }
    
    //Swap Logic

    [Client]
    void SwapMode (bool cursorActive) {
        if (cursorActive) {
            InputHandler.DeRegLookAndShoot ();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            nonCursor = !nonCursor;
        } else {
            InputHandler.ReRegLookAndShoot ();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            nonCursor = !nonCursor;
        }

        UI_Manager.instance.ToggleBtnsInteractivity (cursorActive);

    }
    
    //Crosshair Logic
    
    [Client]
    void UpdateCrosshairPosition (Vector2 lookVector) {
        if (chInstance) {
            CmdUpdateCrosshairPosition (lookVector);
        }
    }
    
    [Command]
    void CmdUpdateCrosshairPosition (Vector2 lookVector) {
        RpcUpdateCrosshairPosition (lookVector);
    }
    
    [ClientRpc]
    void RpcUpdateCrosshairPosition (Vector2 lookVector) {
        if (chInstance) {
            Vector3 screenPosition = new Vector3 (lookVector.x, lookVector.y, 0);
            ((RectTransform)chInstance.transform).position = screenPosition;
        }
    }

    [ClientRpc]
    public void ToggleCrosshair() {
        bool isActive = chInstance.gameObject.activeSelf; // Check current active state
        chInstance.gameObject.SetActive(!isActive); // Toggle crosshair visibility
        
        if (ScoreManager.instance.gameActive) {
            SwapMode(true);
        } else {
            SwapMode(!nonCursor);  
        }
        

        // Subscribe or unsubscribe based on the new state
        if (isActive) {
            // If it was active before, we unsubscribe to prevent issues
            InputHandler.OnSwapPerformed -= InputHandlerOnOnSwapPerformed;  
        } else {
            // If it was inactive, we subscribe to the event
            InputHandler.OnSwapPerformed += InputHandlerOnOnSwapPerformed;  
        }
    }
    

    //Color change logic
    [Server]
    public void SetColor () {
        if (chInstance != null) {
            Color color = Color.white;
            int index = PlayerManager.instance.playersList.IndexOf (this);
            switch (index) {
                case 0:
                    color = Color.red;
                    break;
                case 1:
                    color = Color.green;
                    break;
                case 2:
                    color = Color.blue;
                    break;
                case 3:
                    color = Color.yellow;
                    break;
                // Add more cases if you have more players
            }

            ChangeColor (color); // Change color of Game obj on the server
        }
    }

    [Server]
    void ChangeColor (Color color) {
        playerColor = color;
    }

    [Client]
    void OnColorChanged (Color oldColor, Color newColor) {
        if (playerRenderer) {
            playerRenderer.material.color = newColor;
        }

        if (chInstance) {
            chInstance.SetColor (newColor);
        }

        if (scoreInstance) {
            scoreInstance.SetColor (newColor);
        }

        if (!isLocalPlayer) return;
        if (UI_Manager.instance) {
            HealthManager.instance.ChangeHeartsColor (newColor);
        }

    }

    [Server]
    public Color GetColor () {
        return playerColor;
    }
    
    //Player spawn points logic
    [Server]
    void SetPlaySpawnPos () {
        int index = PlayerManager.instance.playersList.IndexOf (this);

        transform.position = SpawnPointManager.Instance.GetSpawnPoint (index).position;
        TargetSetPlayerSpawnPos (transform.position);
    }

    [TargetRpc]
    void TargetSetPlayerSpawnPos (Vector3 pos) {
        transform.position = pos;
    }

    //Look input handling
    
    [Client]
    void HandleLook() {
        Vector2 lookVector = InputHandler.LookVector;
        UpdateCrosshairPosition(lookVector);

        Ray ray = Camera.main.ScreenPointToRay(InputHandler.LookVector);
        RaycastHit hit;

        // Perform the raycast and check if it hits anything
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~playerMask)) {
            // If it hits something, look at the hit point
            Vector3 lookAtPoint = hit.point;

            // Smoothly interpolate the look direction
            StartCoroutine(LerpLookAt(lookAtPoint));

            // Command to handle the look point on the server
            CmdHandleLook(lookAtPoint);
        }
    }

    [Client]
    IEnumerator LerpLookAt(Vector3 target) {
        // Define the duration of the lerp
        float duration = 0.1f;
        float elapsedTime = 0f;
        Vector3 initialDirection = transform.forward;

        // Perform the lerp over time
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            Vector3 newDirection = Vector3.Lerp(initialDirection, (target - transform.position).normalized, t);
            transform.rotation = Quaternion.LookRotation(newDirection);
            yield return null;
        }

        // Ensure the final rotation is exactly towards the target
        transform.rotation = Quaternion.LookRotation(target - transform.position);
    }

    [Command]
    void CmdHandleLook(Vector3 lookAt) {
        RpcHandleLook(lookAt);
    }

    [ClientRpc]
    void RpcHandleLook(Vector3 lookAt) {
        StartCoroutine(LerpLookAt(lookAt));
    }
    
    //Firing input handling
    
    [Client]
    void HandleFiring(bool firing)
    {
        isFiring = firing;
        if (isFiring)
        {
            if (this != null) // Check if the player object still exists
            {
                StartCoroutine(FiringBullets());
            }
        }
    }

    [Client]
    IEnumerator FiringBullets() {
        while (isFiring && this != null) {
            if (Time.time >= lastFireTime + fireRate) {
                CmdFireBullet();
                lastFireTime = Time.time;
            }
            yield return null; // Adjust to return after a very short delay if necessary.
        }
    }

    [Command]
    void CmdFireBullet () {
        Bullet bullet = BulletPoolManager.Instance.GetBullet (playerColor, this); //change it so it returns bullet instead of gameobject
        if (bullet != null) {
            // Set the bullet's position and rotation
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;

            // Notify clients about the initial position and rotation
            bullet.RpcUpdateTransform (bullet.transform.position, bullet.transform.rotation);

            StartCoroutine (WaitThenEnable (bullet));
        }
    }

    IEnumerator WaitThenEnable (Bullet bullet) {
        yield return 4; //wait 1 frame before enabling
        
        // Ensure the bullet is enabled and starts moving in the correct direction
        bullet.EnableObj ();
    }
    
    //Score logic

    [Server]
    public int GetScore () {
        return playerScore;
    }

    [Command]
    void CmdToggleScoring () {
        ToggleScoring ();
    }

    [Server]
    void ToggleScoring () {
        isScoring = !isScoring;
    }

    [Server]
    public void AddScore (int amount, Player owner) { //adds score to manager that updates clients' local ui
        if (isScoring) {
            ScoreManager.instance.UpdateScore (amount);
            if (owner == this) {
                UpdateScore (amount);
            }
        }
    }

    [Server]
    public void RemoveScoreOnDisconnect () {
        ScoreManager.instance.UpdateScore (-playerScore);
    }

    [Server]
    void UpdateScore (int amount) {
        playerScore += amount;
    }
    
    [Client]
    void OnScoreChanged (int oldScore, int newScore) {
        scoreInstance.SetScore (newScore);
        if (isLocalPlayer) {
            UI_Manager.instance.UpdatePersonalScoreText (newScore);    
        }
    }

    [TargetRpc]
    public void UpdateStats () { //currently not working? users aren't updating their own stats at the end of the game
        if (playerScore < UserProfile.instance.GetCurrentHigh()) { } else {
            UserProfile.instance.SetHigh (playerScore);
            Debug.Log ($"Updated {gameObject.name}'s highscore ");
        }
    }

    //Cooldown based logic including changing fills and calculations
    
    [Client]
    float GetCooldownProgress() //returns ui progress for crosshair fill
    {
        float _timeSinceLastFire = Time.time - lastFireTime;
        return Mathf.Clamp01(_timeSinceLastFire / fireRate);
    }
    
    [Client]
    void ChangeFillAmount (float amount) {
        CmdChangeFillAmount (amount);
    }

    [Command]
    void CmdChangeFillAmount (float amount) {
        RpcChangeFillAmount (amount);
    }
    
    [ClientRpc]
    void RpcChangeFillAmount (float amount) {
        if (chInstance) {
            chInstance.UpdateFill (amount);
        }
    }
    
    // Health Logic

    [Server]
    public void ChangeCollision () {
        if (isServer) {
            playerCol.enabled = !playerCol.enabled;
            RpcChangeColision (); 
        }
    }

    [Command(requiresAuthority = false)]
    void CmdChangeCollision () {
        ChangeCollision ();
    }

    [ClientRpc]
    void RpcChangeColision () {
        playerCol.enabled = !playerCol.enabled;
    }

    [Server]
    public void TakeDamage () {
        currentHealth--;
    }

    [Client]
    void OnHealthChanged (int oldHealth, int newHealth) {
        if (isLocalPlayer) {
            if (currentHealth >= 0 && newHealth < oldHealth) {   //SPECIFICALLY LOOK IF HEALTH WAS LOST NOT GAINED SO WHEN CURRENT HEALTH IS SET BACK TO MAX IT DOESN'T REMOVE A HEART
                HealthManager.instance.RemoveAHeart();
            }
            
            if (currentHealth <= 0) {
                InputHandler.Disable();
                CmdChangeCollision();
                StartCoroutine(WaitBeforeReEnable());
            } 
        }
    }


    //Respawn Logic

    [Command(requiresAuthority = false)]
    void CmdRespawn() {
        Respawn ();
    }

    [Server]
    void Respawn () {
        currentHealth = maxHealth;
    }
    

    [Client]
    IEnumerator WaitBeforeReEnable () {
        yield return new WaitForSeconds (10);
        InputHandler.Enable ();
        CmdChangeCollision ();
        CmdRespawn ();
        HealthManager.instance.ResetHearts();
    }
    
    
    //Player sync on other player Disconnect

    [Server]
    public void SyncMe () {
        SetPlaySpawnPos ();
        SetColor ();
    }
    
    //Player logic to reset after restart btn press

    public void ResetMe () {
        Respawn (); //resets health
        playerScore = 0; //resets score
    }

}