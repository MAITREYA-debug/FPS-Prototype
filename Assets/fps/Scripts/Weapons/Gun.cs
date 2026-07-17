using System.Collections;
using TMPro;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{

    [SerializeField] public GunData gunData;
    [SerializeField] public Player_Controller playerController;
    [SerializeField] public Camera camera;
    [SerializeField] TextMeshProUGUI AmmoTxt;

    [Header("Firing")]
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] public GameObject muzzleFlash;
    [SerializeField] public GameObject hitImpact;
    [SerializeField] public Transform muzzleFlashPosition;


    [Header("Current gun")]
    public float currentBullets = 0;
    public float fireDelayCounter = 0;
    public bool isReloading = false;

    public void Awake()
    {
        playerController = transform.root.GetComponent<Player_Controller>();
        camera = playerController.GetComponentInChildren<Camera>();

        currentBullets = gunData.magzine;

        muzzleFlashPosition = this.transform.Find("muzzleFlashPosition");
        if(muzzleFlashPosition == null)
        {
            Debug.LogError("MuzzleFlashPosition not found in " + this.name);
        }
    }

      

    public void Update()
    {
        
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

        return camera.ViewportPointToRay(new Vector3(0.5f + spreadX, 0.5f + spreadY, 0f));
    }


    public void tryReload()
    {
        if (currentBullets < gunData.magzine)
        {
            StartCoroutine(Reloading());
        }
    }
    

    public IEnumerator Reloading()
    {

        isReloading = true;

        yield return new WaitForSeconds(gunData.reloadTime);

        currentBullets = gunData.magzine;
        isReloading = false;

        Debug.Log("Gun is reloaded");

    }

    void UpdateAmmoDisplay()
    {
        if (AmmoTxt != null)
        {
            AmmoTxt.SetText(currentBullets.ToString());
        }
    }
}
