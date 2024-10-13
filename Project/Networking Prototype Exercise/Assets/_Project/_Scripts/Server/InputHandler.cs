using System;
using UnityEngine;
using UnityEngine.InputSystem;

public static class InputHandler {

    static ClientInputs clientInputs;
    public static Vector2 MovementVector;
    
    public static event EventHandler<Vector2> OnMovementPerformed; //less trash buildup then using stock event. event makes it only able to be invoked in this class
    public static event EventHandler OnFireStarted;
    public static event EventHandler OnFireCanceled;

    public static void Enable () {
        clientInputs ??= new ClientInputs ();
        clientInputs?.Enable ();
        RegisterInputs ();
    }

    public static void Disable () {
        clientInputs?.Disable ();
    }

    public static void Dispose () {
        clientInputs?.Dispose ();
        clientInputs = null;
    }

    static void RegisterInputs () {
        clientInputs.Player.Move.performed += ClientMovementPerformed;
        clientInputs.Player.Move.canceled += ClientMovementCanceled;
        
        clientInputs.Player.Attack.started += AttackOnstarted;
        clientInputs.Player.Attack.canceled += AttackOncanceled;
    }

    static void AttackOncanceled (InputAction.CallbackContext obj) {
        OnFireCanceled?.Invoke(null, EventArgs.Empty);
    }

    static void ClientMovementCanceled (InputAction.CallbackContext obj) {
        
    }

    static void AttackOnstarted (InputAction.CallbackContext obj) {
       OnFireStarted?.Invoke(null, EventArgs.Empty);
    }

    static void ClientMovementPerformed (InputAction.CallbackContext ctx) {
        Vector2 _movementVector = ctx.ReadValue<Vector2> ();
        MovementVector = _movementVector;
        OnMovementPerformed?.Invoke (null, _movementVector);
    }
}
