using System;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon_Controller : MonoBehaviour
{

    [Header("Refferenc")]
    [SerializeField] private Player_Controller playerController;
        bool isInitialized = false;
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] WeaponClass currentWeapon;

    [Header("Setting")]
    [SerializeField] float lookrotationAmount = 3f;
    [SerializeField] float MoveRotationAmount = 5f;
    [SerializeField] float smoothSpeed = 15f;

    [SerializeField] float xClampRotation = 15f;
    [SerializeField] float yClampRotation = 15f;
    [SerializeField] float zClampRotation = 15f;

    [Header("Weapon Breathing")]
    [SerializeField]  Transform WeaponSwayObject;
    [SerializeField]  float swayAmountA = 1;
    [SerializeField]  float swayAmountB = 2;
    [SerializeField]  float swayScale = 600;
    [SerializeField]  float swaylerpSpeed = 14;
    Vector3 swayPosition;
    float swayTime;

    [Header("AimingIn")]
    [SerializeField] bool isAimingIn;
    [SerializeField] float smoothTime;
    [SerializeField] Transform ScopeCameraPosition;
    public float AimOffset;
    Vector3 aimVelocity;
    Vector3 weaponAimPosition;

    [Header("fireing")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform BulletSpawnPoint;
    [SerializeField] float fireRate;
    [SerializeField] float currentFireRate;
    [SerializeField] bool isFiring;

   

    Vector2 lookInput;
    Vector3 LookTarget = Vector3.zero;
 
    Vector2 MovementInput;
    Vector3 MoveTarget  = Vector3.zero;
    
    Quaternion newTarget;




    private void Awake()
    {
        weaponAnimator = GetComponentInChildren<Animator>();
    }

    public void Initialization(Player_Controller playercontroller)
    {
        playerController = playercontroller;
        isInitialized = true;
    }

    #region - onEnable/ OnDisable -
    private void OnEnable()
    {      
     
    }
    private void OnDisable()
    {    

    }

    #endregion

    private void Update()
    {
        if (!isInitialized)
        {
            return;
        }
        Sway_Look_Calculation();
        Sway_Idle_Calculation();
        isAiming_Calculation();
        Shoot();
        //Debug.Log(playerController.weaponAnimationMagnitude);
        weaponAnimator.SetFloat("Speed", playerController.weaponAnimation_Speed);
        weaponAnimator.SetBool("Sprinting" , InputManager.instance.isSprinting);
        weaponAnimator.SetBool("isGrounded", playerController.isGrounded);
    }

    void LateUpdate()
        {
             
        }

    #region -isAiming calculation-
    void isAiming_Calculation()
    {
        isAimingIn = InputManager.instance.isAimingIn;

        Vector3 target = transform.position;

        if (isAimingIn){

            target = playerController.cameraHolder.transform.position 
                + (WeaponSwayObject.transform.position - ScopeCameraPosition.position) 
                + playerController.cameraHolder.transform.forward * AimOffset;        
        
        }

        weaponAimPosition = WeaponSwayObject.transform.position;
        weaponAimPosition = Vector3.SmoothDamp(weaponAimPosition, target, ref aimVelocity, smoothTime);

        WeaponSwayObject.transform.position = weaponAimPosition + swayPosition;
    }

    #endregion

    #region - Sway Look Cal -
    void Sway_Look_Calculation()
    {
        lookInput = InputManager.instance.LookInput;
        MovementInput = InputManager.instance.MoveInput;



        LookTarget.x = lookInput.y * (isAimingIn ? lookrotationAmount / 4 : lookrotationAmount);
        LookTarget.y = -lookInput.x * (isAimingIn ? lookrotationAmount / 4 : lookrotationAmount);
        LookTarget.z = -lookInput.x / 2;

        MoveTarget.z = MovementInput.x * (isAimingIn ? MoveRotationAmount / 4 : MoveRotationAmount);
        MoveTarget.x = -MovementInput.y * (isAimingIn ? MoveRotationAmount / 4 : MoveRotationAmount);

        Vector3 target = LookTarget + MoveTarget;

        newTarget = Quaternion.Euler(Math.Clamp(target.x, -xClampRotation, xClampRotation),
                Math.Clamp(target.y, -yClampRotation, yClampRotation),
                Math.Clamp(target.z, -zClampRotation, zClampRotation));

        transform.localRotation = Quaternion.Slerp(transform.localRotation, newTarget, smoothSpeed * Time.deltaTime);
    }

    #endregion

    #region - Sway Idle Calculation - 
    private void Sway_Idle_Calculation()
    { 
        var targetPos = LissajousCurve(swayTime, swayAmountA, swayAmountB) / ( isAimingIn ? swayScale * 4: swayScale) ;

        swayTime += Time.deltaTime;
        if (swayTime > 6.3f) swayTime = 0;

        swayPosition = Vector3.Lerp(swayPosition, targetPos, Time.deltaTime * swaylerpSpeed);
        //WeaponSwayObject.localPosition = swayPosition;
    }
    private Vector3 LissajousCurve(float Time, float A, float B)
    {
        return new Vector3(Mathf.Sin(Time), A * Mathf.Sin(B * Time + Mathf.PI));
    }

    #endregion

    #region - jumpevents -

    public void onjump()
    {
        weaponAnimator.SetTrigger("OnJump");
       
    } 

   public void Falling()
    {
        weaponAnimator.SetTrigger("Falling");
    }

    public void OnLanding()
    {
        weaponAnimator.SetTrigger("OnLanding");
    }

    #endregion

    #region
    void Shoot()
    {
        isFiring = InputManager.instance.isFiring;
        if (isFiring)
        {
            if(currentFireRate > fireRate)
            {
                var other = Instantiate(bulletPrefab, BulletSpawnPoint.position , Quaternion.identity);
                currentFireRate = 0;
            }
            currentFireRate += Time.deltaTime;
        }
    }
    #endregion

}