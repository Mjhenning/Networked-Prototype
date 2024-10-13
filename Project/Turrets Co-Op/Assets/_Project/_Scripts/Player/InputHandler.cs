using System;
using UnityEngine;
using UnityEngine.InputSystem;

public static class InputHandler
{
    static ClientInputs clientInputs;
    public static Vector2 LookVector;
    public static event EventHandler OnFireStarted;
    public static event EventHandler OnFireCanceled;
    public static event EventHandler<Vector2> OnLookStarted;

    public static event EventHandler OnSwapPerformed; 

    public static void Enable()
    {
        clientInputs ??= new ClientInputs();
        clientInputs?.Enable();
        RegisterInputs();
    }

    public static void Disable()
    {
        clientInputs?.Disable();
    }

    public static void Dispose()
    {
        clientInputs?.Dispose();
        clientInputs = null;
    }

    static void RegisterInputs()
    {
        clientInputs.Player.Attack.started += AttackOnStarted;
        clientInputs.Player.Attack.canceled += AttackOnCanceled;
        clientInputs.Player.Look.performed += LookOnPerformed;

        clientInputs.Player.CursorMode.performed += SwapOnPerformed;
        clientInputs.Player.Quit.performed += QuitOnPerformed;
    }
    

    public static void DeRegLookAndShoot () { //used to stop subscribing to look and attacks
        clientInputs.Player.Look.performed -= LookOnPerformed;
        clientInputs.Player.Attack.started -= AttackOnStarted;
        clientInputs.Player.Attack.canceled -= AttackOnCanceled;
    }
    
    public static void ReRegLookAndShoot () { //used to subscribe to look and attacks
        clientInputs.Player.Look.performed += LookOnPerformed;
        clientInputs.Player.Attack.started += AttackOnStarted;
        clientInputs.Player.Attack.canceled += AttackOnCanceled;
    }
    
    static void QuitOnPerformed (InputAction.CallbackContext obj) { //just a quick force quit bind
        Debug.Log ("Quitting");
        Application.Quit ();
    }

    static void LookOnPerformed(InputAction.CallbackContext ctx)
    {
        Vector2 lookVector = LookVector + ctx.ReadValue<Vector2>();
        
        // Constrain the look vector within the screen bounds
        lookVector.x = Mathf.Clamp(lookVector.x, 0, Screen.width);
        lookVector.y = Mathf.Clamp(lookVector.y, 0, Screen.height);
        
        LookVector = lookVector;
        OnLookStarted?.Invoke(null, lookVector);
    }

    static void SwapOnPerformed (InputAction.CallbackContext ctx) {
        OnSwapPerformed?.Invoke (null, EventArgs.Empty);
    }
    

    static void AttackOnCanceled(InputAction.CallbackContext obj)
    {
        OnFireCanceled?.Invoke(null, EventArgs.Empty);
    }

    static void AttackOnStarted(InputAction.CallbackContext obj)
    {
        OnFireStarted?.Invoke(null, EventArgs.Empty);
    }
}