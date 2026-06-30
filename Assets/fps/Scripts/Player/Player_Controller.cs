using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player_Controller : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] public Transform cameraHolder;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Weapon_Controller weapon_Controller;

    [Header("Movement")]
    [SerializeField] float currentSpeed;
    [SerializeField] float speed;
    [SerializeField] float currentSpeedMultiplier = 1;
    [SerializeField] float walkSpeed = .8f;
    [SerializeField] float runSpeed = 1f;
    [SerializeField] private float crouchSpeed = 0.4f;
    [SerializeField] float fallSpeed = 0.5f;
    [SerializeField] float aimSpeed =  0.3f;    
    [SerializeField] float _jumpHeight = 1.5f;
    [SerializeField] float _gravity = -50f;
    bool _isRunning;
    bool isFalling;
    bool _jump = false;
    float _verticalVelocity;
    bool wasGrounded;

    Vector3 defaultCamerapos;


    [Header("Crouch")]
    [SerializeField] private float _crouchHeight = 1.5f;
    [SerializeField] private float _standingHeight = 2f;
    private bool _isCrouching;

    [Header("Look")]
    [SerializeField] private float _pitch;
    [SerializeField] private float _mouseSensitivity = 0.3f;
    [SerializeField] private float _maxlookAngle = 0.60f;


    [Header("Weapon")]
    public float weaponAnimation_Speed = 0;
    public bool isGrounded;

    public bool isAimingIn;


    enum SpeedState
    {
        walkState,
        runState,
        crouchState,
        fallState,
        aimingState
    }

    private SpeedState currentSpeedState = SpeedState.walkState;
    Vector3 offset;

    void Awake()
    {
        weapon_Controller = this.GetComponentInChildren<Weapon_Controller>();
        weapon_Controller.Initialization(this);

        playerAnimator = GetComponent<Animator>();
        if (playerAnimator == null) Debug.LogError("_animator is null");

        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("_character Controller is null");
        }

        currentSpeed = walkSpeed;
        _verticalVelocity = 0f;
        defaultCamerapos = cameraHolder.transform.localPosition;
        offset = defaultCamerapos - characterController.center;
    }

#region- enable/disable-
    void OnEnable()
    {
        InputManager.OnJump += jump;


    }
    void OnDisable()
    {
        InputManager.OnJump -= jump;

    }
    #endregion


    void Update()
    {
        StateManager();
        JumpEvents();
        HandleMovement();
        HandleLook();
        HandleCrouch();
    }

    #region - stateManager -
    void StateManager()
    {
        _isRunning = InputManager.instance.isSprinting;
        _isCrouching = InputManager.instance.isCrouching;
        isAimingIn = InputManager.instance.isAimingIn;

        if (isFalling)
        {
            currentSpeedState = SpeedState.fallState;
        }
        else
        {
            currentSpeedState = isAimingIn ? SpeedState.aimingState : _isCrouching ? SpeedState.crouchState : _isRunning ? SpeedState.runState : SpeedState.walkState;
        }
        
    
        switch (currentSpeedState)
        {
            case SpeedState.crouchState:
                currentSpeedMultiplier = crouchSpeed;
                break;
            case SpeedState.runState:
                currentSpeedMultiplier = runSpeed;
                break ;
            case SpeedState.walkState:
                currentSpeedMultiplier = walkSpeed;
                break;
            case SpeedState.fallState:
                currentSpeedMultiplier = fallSpeed;
                break;
            case SpeedState.aimingState:
                currentSpeedMultiplier = aimSpeed;
                break;
            default:
                currentSpeedMultiplier = walkSpeed;
                break;
        }
    
    }
    #endregion

    #region - Movement -
    void HandleMovement()
    {

        currentSpeed = speed * currentSpeedMultiplier;
        Vector2 moveInput = InputManager.instance.MoveInput;
        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y) * currentSpeed;

        if (moveInput.y != 1f && _isRunning == true)
        {
            _isRunning = false;

        }
        if (characterController.isGrounded)
        {
            if (_jump)
            {
                Debug.Log("Jump");

                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                _jump = false;
            }
            else
            {
                _verticalVelocity = -2f;
            }
        }
        _verticalVelocity += _gravity * Time.deltaTime;
        move.y += _verticalVelocity;

        characterController.Move(move * Time.deltaTime);

        weaponAnimation_Speed = characterController.velocity.magnitude / currentSpeed;
        if (weaponAnimation_Speed > 1) weaponAnimation_Speed = 1;
        if (weaponAnimation_Speed < 0.05) weaponAnimation_Speed = 0;


    }

    #endregion

    #region - look -
    void HandleLook()
    {
        Vector2 mouseInput = InputManager.instance.LookInput * _mouseSensitivity;

        transform.Rotate(transform.up * mouseInput.x);

        _pitch -= mouseInput.y;
        _pitch = Mathf.Clamp(_pitch, -_maxlookAngle, _maxlookAngle);
        cameraHolder.localRotation = Quaternion.Euler(_pitch, 0, 0);


    }

    #endregion

    #region - Crouch -
    void HandleCrouch()
    {
        float targetHeight = _isCrouching ? _crouchHeight : _standingHeight;

        characterController.height = Mathf.Lerp(characterController.height, targetHeight, 10f * Time.deltaTime/* speed */ );

        // the Center Sets on Foot of the Player 
        Vector3 center = characterController.center;
        center.y = characterController.height * 0.5f;
        characterController.center = center;

        

        cameraHolder.localPosition = center + offset;
       
    }
    #endregion

    #region  - Jump -
    void jump()
    {
        _jump = true;
       
    }
    float falltime;
    void JumpEvents()
    {
        isGrounded = characterController.isGrounded;

        // Jump
        if (isGrounded && _jump)
        {
            Debug.Log("jumpstart");
            weapon_Controller.onjump();
        }

        // Started Falling
        if (!isGrounded)
        {
            falltime += Time.deltaTime;

            if (falltime > 0.15f && characterController.velocity.y < 0 && wasGrounded && !isFalling)
            {
                Debug.Log("Falling");
                isFalling = true;
                weapon_Controller.Falling();
            }
           

        }
        else
        {
            falltime = 0;
        }


        // Landed
        if (!wasGrounded && isGrounded)
        {
            Debug.Log("landing");
            isFalling = false;
            weapon_Controller.OnLanding();
        }

        wasGrounded = isGrounded;
    }

    #endregion



}

