using System;
using UnityEngine;

public class Weapon_Controller : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Player_Controller playerController;
    bool isInitialized;

    [SerializeField] Animator weaponAnimator;
    [SerializeField] Transform WeaponSwayObject;
    [SerializeField] Transform ScopeCameraPosition;

    [Header("Sway")]
    [SerializeField] float lookrotationAmount = 3f;
    [SerializeField] float MoveRotationAmount = 5f;
    [SerializeField] float smoothSpeed = 15f;

    [SerializeField] float xClampRotation = 15f;
    [SerializeField] float yClampRotation = 15f;
    [SerializeField] float zClampRotation = 15f;

    [Header("Weapon Breathing")]
    [SerializeField] float swayAmountA = 1f;
    [SerializeField] float swayAmountB = 2f;
    [SerializeField] float swayScale = 600f;
    [SerializeField] float swaylerpSpeed = 14f;

    [Header("Aiming")]
    [SerializeField] float smoothTime = 0.1f;
    public float AimOffset;

    Vector3 swayPosition;
    float swayTime;
    Vector3 aimVelocity;
    Vector3 weaponAimPosition;

    Vector2 lookInput;
    Vector3 LookTarget = Vector3.zero;

    Vector2 MovementInput;
    Vector3 MoveTarget = Vector3.zero;

    Quaternion newTarget;

    void Awake()
    {
        weaponAnimator = GetComponentInChildren<Animator>();
    }

    public void Initialization(Player_Controller playercontroller)
    {
        playerController = playercontroller;
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized || InputManager.instance == null || playerController == null)
        {
            return;
        }

        Sway_Look_Calculation();
        Sway_Idle_Calculation();
        isAiming_Calculation();

        if (weaponAnimator != null)
        {
            weaponAnimator.SetFloat("Speed", playerController.weaponAnimation_Speed);
            weaponAnimator.SetBool("Sprinting", InputManager.instance.isSprinting);
            weaponAnimator.SetBool("isGrounded", playerController.isGrounded);
        }
    }

    #region - Aiming -
  

    void isAiming_Calculation()
    {
        bool isAimingIn = InputManager.instance.isAimingIn;
        Vector3 target = transform.position;

        if (isAimingIn && WeaponSwayObject != null && ScopeCameraPosition != null)
        {
            target = playerController.cameraHolder.transform.position
                + (WeaponSwayObject.transform.position - ScopeCameraPosition.position)
                + playerController.cameraHolder.transform.forward * AimOffset;
        }

        if (WeaponSwayObject == null)
        {
            return;
        }

        weaponAimPosition = WeaponSwayObject.transform.position;
        weaponAimPosition = Vector3.SmoothDamp(weaponAimPosition, target, ref aimVelocity, smoothTime);
        WeaponSwayObject.transform.position = weaponAimPosition + swayPosition;
    }
    #endregion

    #region - Sway_Look -
    
    void Sway_Look_Calculation()
    {
        bool isAimingIn = InputManager.instance.isAimingIn;
        lookInput = InputManager.instance.LookInput;
        MovementInput = InputManager.instance.MoveInput;

        float lookMultiplier = isAimingIn ? lookrotationAmount / 4f : lookrotationAmount;
        float moveMultiplier = isAimingIn ? MoveRotationAmount / 4f : MoveRotationAmount;

        LookTarget.x = lookInput.y * lookMultiplier;
        LookTarget.y = -lookInput.x * lookMultiplier;
        LookTarget.z = -lookInput.x / 2f;

        MoveTarget.z = MovementInput.x * moveMultiplier;
        MoveTarget.x = -MovementInput.y * moveMultiplier;

        Vector3 target = LookTarget + MoveTarget;

        newTarget = Quaternion.Euler(
            Math.Clamp(target.x, -xClampRotation, xClampRotation),
            Math.Clamp(target.y, -yClampRotation, yClampRotation),
            Math.Clamp(target.z, -zClampRotation, zClampRotation));

        transform.localRotation = Quaternion.Slerp(transform.localRotation, newTarget, smoothSpeed * Time.deltaTime);
    }
    #endregion

    #region - Sway idle -
    void Sway_Idle_Calculation()
    {
        bool isAimingIn = InputManager.instance.isAimingIn;
        float scale = isAimingIn ? swayScale * 4f : swayScale;
        var targetPos = LissajousCurve(swayTime, swayAmountA, swayAmountB) / scale;

        swayTime += Time.deltaTime;
        if (swayTime > 6.3f)
        {
            swayTime = 0f;
        }

        swayPosition = Vector3.Lerp(swayPosition, targetPos, Time.deltaTime * swaylerpSpeed);
    }

    Vector3 LissajousCurve(float Time, float A, float B)
    {
        return new Vector3(Mathf.Sin(Time), A * Mathf.Sin(B * Time + Mathf.PI));
    }

    #endregion

    #region - jump Events -
    public void onjump()
    {
        weaponAnimator?.SetTrigger("OnJump");
    }

    public void Falling()
    {
        weaponAnimator?.SetTrigger("Falling");
    }

    public void OnLanding()
    {
        weaponAnimator?.SetTrigger("OnLanding");
    }
    #endregion

  


}
