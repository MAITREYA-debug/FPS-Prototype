using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player_Controller : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera camera;
    [SerializeField] public Transform cameraHolder;
    [SerializeField] private Weapon_Controller weapon_Controller;

    [Header("Movement")]
    [SerializeField] float currentSpeed;
    [SerializeField] float speed;
    [SerializeField] float currentSpeedMultiplier = 1;
    [SerializeField] float walkSpeed = .8f;
    [SerializeField] float runSpeed = 1f;
    [SerializeField] private float crouchSpeed = 0.4f;
    [SerializeField] float fallSpeed = 0.5f;
    [SerializeField] float aimSpeed = 0.3f;
    [SerializeField] float _jumpHeight = 1.5f;
    [SerializeField] float _gravity = -50f;

    bool _isRunning;
    bool isFalling;
    bool _jump;
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
    public float weaponAnimation_Speed;
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
        weapon_Controller = GetComponentInChildren<Weapon_Controller>();
        if (weapon_Controller == null)
        {
            Debug.LogWarning($"{nameof(Player_Controller)} could not find {nameof(Weapon_Controller)} in children.", this);
        }
        else
        {
            weapon_Controller.Initialization(this);
        }

        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError($"{nameof(CharacterController)} is missing.", this);
        }

        camera = this.GetComponentInChildren<Camera>();
        if(camera == null)
        {
            Debug.LogError($"{nameof(Camera)} is missing.", this);
        }

        currentSpeed = walkSpeed;
        _verticalVelocity = 0f;
        defaultCamerapos = cameraHolder.transform.localPosition;
        offset = defaultCamerapos - characterController.center;
    }


    void Update()
    {
        if (InputManager.instance == null)
        {
            return;
        }

        StateManager();
        JumpEvents();
        HandleMovement();
        HandleLook();
        HandleCrouch();
    } 

    #region - Enable/Disable -
    void OnEnable()
    {
        InputManager.OnJump += jump;
        InputManager.onPickup += WeaponPickInteraction;
    }

    void OnDisable()
    {
        InputManager.OnJump -= jump;
        InputManager.onPickup -= WeaponPickInteraction;
    }
#endregion

    #region - Movement -
    void StateManager()
    {
        Vector2 moveInput = InputManager.instance.MoveInput;
        _isRunning = InputManager.instance.isSprinting && moveInput.y > 0f;
        _isCrouching = InputManager.instance.isCrouching;
        isAimingIn = InputManager.instance.isAimingIn;

        if (isFalling)
        {
            currentSpeedState = SpeedState.fallState;
        }
        else
        {
            currentSpeedState = isAimingIn ? SpeedState.aimingState
                : _isCrouching ? SpeedState.crouchState
                : _isRunning ? SpeedState.runState
                : SpeedState.walkState;
        }

        switch (currentSpeedState)
        {
            case SpeedState.crouchState:
                currentSpeedMultiplier = crouchSpeed;
                break;
            case SpeedState.runState:
                currentSpeedMultiplier = runSpeed;
                break;
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

        if (characterController.isGrounded)
        {
            if (_jump)
            {
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

        weaponAnimation_Speed = currentSpeed > 0f
            ? characterController.velocity.magnitude / currentSpeed
            : 0f;
        weaponAnimation_Speed = Mathf.Clamp(weaponAnimation_Speed, 0f, 1f);
        if (weaponAnimation_Speed < 0.05f)
        {
            weaponAnimation_Speed = 0f;
        }
    }
    #endregion

    #region - Look -
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

        characterController.height = Mathf.Lerp(characterController.height, targetHeight, 10f * Time.deltaTime);

        Vector3 center = characterController.center;
        center.y = characterController.height * 0.5f;
        characterController.center = center;

        cameraHolder.localPosition = center + offset;
    }
    #endregion

    #region - jump -
    void jump()
    {
        _jump = true;
    }

    float falltime;

    void JumpEvents()
    {
        isGrounded = characterController.isGrounded;

        if (isGrounded && _jump && weapon_Controller != null)
        {
            weapon_Controller.onjump();
        }

        if (!isGrounded)
        {
            falltime += Time.deltaTime;

            if (falltime > 0.15f && characterController.velocity.y < 0 && wasGrounded && !isFalling)
            {
                isFalling = true;
                weapon_Controller?.Falling();
            }
        }
        else
        {
            falltime = 0;
        }

        if (!wasGrounded && isGrounded)
        {
            isFalling = false;
            weapon_Controller?.OnLanding();
        }

        wasGrounded = isGrounded;
    }

    #endregion


    public void WeaponPickInteraction()
    {
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f , 0.5f , 0f));
        

        if (Physics.Raycast(ray, out RaycastHit hit, 4f , LayerMask.GetMask("Player")))
        {
            Debug.Log("weapon pickup hit" + hit.collider.name);
            var obj = hit.collider.GetComponent<Gun>();

            if (obj != null)
            {
                Weapon_Manager.instance.PickUp_Weapon(obj);
                Destroy(obj.gameObject);
            }
        }
    }
}
