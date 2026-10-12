using System.Collections;
using TMPro;
//using UnityEditor.Animations;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{

    [SerializeField] public GunData gunData;
    [SerializeField] public Player_Controller playerController;
    [SerializeField] public Camera Playercamera;
    [SerializeField] TextMeshProUGUI AmmoTxt;
    [SerializeField] public Animator animator;
    [SerializeField] public RuntimeAnimatorController playerAnimationtype;
    [SerializeField] public Transform WeaponLeftHandpos;
    [SerializeField] public Transform WeaponRightHandpos;

    [Header("Firing")]
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] public GameObject muzzleFlash;
    [SerializeField] public GameObject hitImpact;
    [SerializeField] public Transform muzzleFlashPosition;
    [SerializeField] public Transform ScopePosition;



    [Header("Current gun")]
    public int currentBullets = 0;
    public float fireDelayCounter = 0;
    public bool isReloading = false;

    public void Awake()
    {
        playerController = transform.root.GetComponent<Player_Controller>();
        Playercamera = playerController.GetComponentInChildren<Camera>();
        WeaponLeftHandpos = gameObject.transform.Find("WeaponLeft_Hand");
        WeaponRightHandpos = gameObject.transform.Find("WeaponRight_Hand");

        currentBullets = gunData.magazine;

        muzzleFlashPosition = this.transform.Find("muzzleFlashPosition");
        if(muzzleFlashPosition == null)
        {
            Debug.LogError("MuzzleFlashPosition not found in " + this.name);
        }

        animator = transform.GetComponent<Animator>();
        ScopePosition = transform.Find("InSight").GetComponent<Transform>();
    }

      

    virtual
    public void tryShoot()
    {
        fireDelayCounter += Time.deltaTime;

        if (!InputManager.instance.isFiring || currentBullets <= 0) return;

        if (fireDelayCounter < gunData.FireRate) return;
      
        if (isReloading) return;

        shoot();

    }

    public abstract void shoot();


    public Ray BuildAimRay(bool applySpread)
    {
        float spreadX = 0f;
        float spreadY = 0f;

        if (applySpread && gunData.ErrorRange > 0f)
        {
            spreadX = (Random.value - 0.5f) * 2f * gunData.ErrorRange / Screen.width;
            spreadY = (Random.value - 0.5f) * 2f * gunData.ErrorRange / Screen.height;
        }

        return Playercamera.ViewportPointToRay(new Vector3(0.5f + spreadX, 0.5f + spreadY, 0f));
    }


    public void tryReload()
    {
        if (currentBullets < gunData.magazine)
        {
            StartCoroutine(Reloading());
        }
    }
    

    public IEnumerator Reloading()
    {

        isReloading = true;

        yield return new WaitForSeconds(gunData.reloadTime);

        currentBullets = gunData.magazine;
        isReloading = false;

        Debug.Log("Gun is reloaded");

    }

    public void UpdateAmmoDisplay()
    {
        if (AmmoTxt != null)
        {
            AmmoTxt.SetText(currentBullets.ToString());
        }
    }
}
