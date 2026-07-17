using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance { get; private set; }

    private InputActions input;

    public Vector2 MoveInput => input.Player.Movement.ReadValue<Vector2>();
    public Vector2 LookInput => input.Player.Look.ReadValue<Vector2>();

    public bool isSprinting { get; private set; }
    public bool isCrouching { get; private set; }
    public bool isAimingIn { get; private set; }
    public bool isFiring { get; private set; }

    public static event Action OnJump;
    public static event Action OnInteract;
    public static event Action OnReload;
    public static event Action OnFire;
    public static event Action<int> OnWeaponSelected;
    public static event Action onPickup;
    public static event Action onDrop;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        input = new InputActions();


        // Player

        input.Player.Sprint.performed += _ => isSprinting = true;
        input.Player.Sprint.canceled += _ => isSprinting = false;

        input.Player.Crouch.performed += _ => isCrouching = true;
        input.Player.Crouch.canceled += _ => isCrouching = false;

        input.Player.Jump.performed += Jump_performed;
        input.Player.Interact.performed += Interact_performed;



        // Weapon
        input.Weapon.Fire2Press.performed += _ => isAimingIn = true;
        input.Weapon.Fire2Release.performed += _ => isAimingIn = false;

        input.Weapon.Fire1Press.performed += _ => isFiring = true;
        input.Weapon.Fire1Press.performed += Fire1Press_performed;
        input.Weapon.Fire1Release.performed += _ => isFiring = false;

        input.Weapon.Reload.performed += Reload_performed;

        input.Weapon.PrimarySeleted.performed += _ => SelectWeapon(1);
        input.Weapon.SecondarySeleted.performed += _ => SelectWeapon(2);
        input.Weapon.MeleeSeleted.performed += _ => SelectWeapon(3);

        input.Weapon.WeaponPickUp.performed += weaponPickup_performed;
        input.Weapon.WeaponDrop.performed += weaponDrop_performed;

        input.Enable();
    }

   

    private void OnDestroy()
    {
        if (instance != this)
        {
            return;
        }

        input?.Disable();
        input?.Dispose();
        instance = null;
    }

    public void SelectWeapon(int id)
    {
        OnWeaponSelected?.Invoke(id);
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
    private void Fire1Press_performed(InputAction.CallbackContext obj)
    {
        OnFire?.Invoke();
    }

    private void weaponPickup_performed(InputAction.CallbackContext obj)
    {
        onPickup?.Invoke();
    }

    private void weaponDrop_performed(InputAction.CallbackContext obj)
    {
        onDrop?.Invoke();
    }
}
