using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public static InputManager instance;
    
    

    private InputActions input;

    public Vector2 MoveInput => input.Player.Movement.ReadValue<Vector2>();
    public Vector2 LookInput => input.Player.Look.ReadValue<Vector2>();

    public bool isSprinting { private set; get; }
    public bool isCrouching { private set; get; }
    public bool isAimingIn { private set; get; }
    public bool isFiring { private set; get; }


   
    public static event Action OnJump;
    public static event Action OnInteract;
    public static event Action OnReload;
         
        private void Awake()
    {
        instance = this;
        input = new InputActions();
        input.Player.Sprint.performed += _ => isSprinting = true;
        input.Player.Sprint.canceled += _ => isSprinting = false;

        input.Player.Crouch.performed += _ => isCrouching = true;
        input.Player.Crouch.canceled += _ => isCrouching = false;

        input.Player.Jump.performed += Jump_performed;
        input.Player.Interact.performed += Interact_performed;


        input.Weapon.Fire2Press.performed += e => isAimingIn = true;
        input.Weapon.Fire2Release.performed += e => isAimingIn = false;

        input.Weapon.Fire1Press.performed += e => isFiring = true;
        input.Weapon.Fire1Release.performed += e => isFiring = false;

        input.Enable();
    }

 

    private void Reload_performed(InputAction.CallbackContext obj)
    {
        OnReload?.Invoke();
    }
     
      

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        OnInteract?.Invoke();
    }

 
    private void Jump_performed(InputAction.CallbackContext obj)
    {
        OnJump?.Invoke();
    }


}

